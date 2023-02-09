using mvdmio.CanaryTest.Web.Interfaces;
using mvdmio.CanaryTest.Web.Results;

namespace mvdmio.CanaryTest.Web;

public class WebCrawler : IAsyncDisposable
{
   private readonly WebCrawlerOptions _options;
   private readonly HttpClient _httpClient;
   private readonly CrawlResults _crawlResults;
   
   private IWebInteractor? _webInteractor;
   private Uri? _currentBaseUri;
   
   public WebCrawler(Uri startUrl) : this(new WebCrawlerOptions {
      Uris = new []{ startUrl }
   }) { }

   public WebCrawler(WebCrawlerOptions options)
   {
      _options = options;

      _crawlResults = new CrawlResults();
      _httpClient = new HttpClient();
   }

   /// <summary>
   /// Initializes the webcrawler.
   /// You can use this method to setup the state of the webcrawler before the run starts.
   /// For instance, to log in with a user account.
   /// </summary>
   public async Task Initialize(Func<IWebInteractor, Task> initializationDelegate)
   {
      _webInteractor ??= await WebInteractorFactory.Create();
      await initializationDelegate.Invoke(_webInteractor);
      await Task.Delay(1000);
   }

   public async Task<CrawlResults> Run()
   {
      _webInteractor ??= await WebInteractorFactory.Create();
      _crawlResults.Clear();

      foreach (var url in _options.Uris)
      {
         _currentBaseUri = url;
         await CheckPageRecursive(url);
      }
      
      Console.WriteLine($"Run finished. Checked {_crawlResults.Checks.Count()} URLs. Failed: {_crawlResults.Failed.Count()}. Succeeded: {_crawlResults.Succeeded.Count()}");

      return _crawlResults;
   }

   private async Task CheckPageRecursive(Uri uri)
   {
      if (_crawlResults.Contains(uri) || _options.IsOnIgnoreList(uri))
         return;
      
      await _webInteractor!.ClearConsole();

      var result = _crawlResults.AddResult(uri);
      try
      {
         await _webInteractor.OpenAndFollowRedirects(uri);
         var consoleErrors = await _webInteractor.GetConsoleErrors();
         result.AddConsoleErrors(consoleErrors);
         
         await CheckLinkTags();
         await CheckScriptTags();
         await CheckAndFollowAnchors();
      }
      catch (Exception ex)
      {
         Console.WriteLine($"Error while checking page: {uri.AbsoluteUri}\r\n{ex}");

         result.AddError(new CheckError {
            Type = "Unknown",
            Message = ex.Message
         });
      }
   }
   
   private async Task CheckLinkTags()
   {
      var linkElements = await _webInteractor!.GetElements("link");

      foreach (var linkElement in linkElements)
      {
         var href = await linkElement.GetAttribute("href");
         var uri = GetAbsoluteUriFromString(href);

         if(uri is null)
            continue;
         
         await CheckUrl(uri, "Link");
      }
   }
   
   private async Task CheckScriptTags()
   {
      var scriptElements = await _webInteractor!.GetElements("script");

      foreach (var scriptElement in scriptElements)
      {
         var src = await scriptElement.GetAttribute("src");
         var uri = GetAbsoluteUriFromString(src);

         if(uri is null)
            continue;

         await CheckUrl(uri, "Script");
      }
   }

   private async Task CheckAndFollowAnchors()
   {
      var anchorElements = await _webInteractor!.GetElements("a");
      foreach (var achorElement in anchorElements)
      {
         var href = await achorElement.GetAttribute("href");
         if (href is not null)
         {
            if (href.StartsWith("javascript"))
            {
               // TODO: What to do with javascript links?
            }
            else
            {
               var uri = GetAbsoluteUriFromString(href);

               if(uri is null)
                  continue;

               if (_options.IsHostAllowed(uri))
               {
                  await CheckPageRecursive(uri);
               }
               else // Don't crawl URLs outside our own host domain. We'd be crawling the entire internet otherwise. Just check if the link works.
               {
                  await CheckUrl(uri, "Anchor");
               }
            }
         }
         else
         {
            // TODO: What to do with links that have no href? Might be an element with javascript handling. Might not be.
         }
      }
   }

   private async Task CheckUrl(Uri uri, string type)
   {
      var result = _crawlResults.AddResult(uri);
      var response = await _httpClient.GetAsync(uri);
      if (!response.IsSuccessStatusCode)
      {
         result.AddError(
            new CheckError {
               Type = type,
               Message = $"{response.StatusCode}: {uri.AbsoluteUri}"
            }
         );
      }
   }

   public async ValueTask DisposeAsync()
   {
      GC.SuppressFinalize(this);

      if (_webInteractor is not null)
         await _webInteractor.DisposeAsync();
   }

   private Uri? GetAbsoluteUriFromString(string? input)
   {
      if (input is null)
         return null;

      try
      {
         if (input.StartsWith("http"))
            return new Uri(input);
      
         return new Uri(_currentBaseUri!, input);
      }
      catch(Exception)
      {
         Console.WriteLine($"Unable to convert {input} into a valid URI");
         return null;
      }
   }
}