using Microsoft.Playwright;
using mvdmio.CanaryTest.Web.Interfaces;

namespace mvdmio.CanaryTest.Web.Playwright;

public sealed class GoogleChromeWebInteractor : IWebInteractor
{
   private readonly IPage _page;
   private readonly List<IConsoleMessage> _consoleMessages;

   private GoogleChromeWebInteractor(IPage page)
   {
      _page = page;

      _consoleMessages = new List<IConsoleMessage>();

      page.Console += RegisterConsoleMessage;
   }
   
   public static async Task<GoogleChromeWebInteractor> Build()
   {
      var playwritght = await Microsoft.Playwright.Playwright.CreateAsync();
      var browser = await playwritght.Chromium.LaunchAsync(new BrowserTypeLaunchOptions {
         Channel = "chrome",
         Headless = false
      });

      var page = await browser.NewPageAsync();
      return new GoogleChromeWebInteractor(page);
   }
   
   public async ValueTask DisposeAsync()
   {
      await _page.Context.DisposeAsync();
   }

   public async Task Open(Uri url)
   {
      await _page.GotoAsync(url.AbsoluteUri);

      if (!_page.Url.Equals(url.AbsoluteUri, StringComparison.InvariantCultureIgnoreCase))
         throw new InvalidOperationException($"Navigating to {url} resulted in redirects to another page. Use {nameof(OpenAndFollowRedirects)} instead of {nameof(Open)} if this is intended.");
   }

   public async Task OpenAndFollowRedirects(Uri url)
   {
      await _page.GotoAsync(url.AbsoluteUri);
   }

   public async Task<IWebElement> GetElementById(string elementId)
   {
      var element = _page.Locator($"id={elementId}");
      return new PlaywrightWebElement(element);
   }

   public async Task<IEnumerable<IWebElement>> GetElements(string elementName)
   {
      var elements = await _page.Locator(elementName).AllAsync();
      return elements.Select(x => new PlaywrightWebElement(x));
   }

   public async Task<IEnumerable<string>> GetConsoleErrors()
   {
      // IConsoleMessage documentation: https://playwright.dev/docs/api/class-consolemessage
      return _consoleMessages.Where(x => x.Type == "error").Select(x => x.Text).ToArray();
   }

   public async Task ClearConsole()
   {
      _consoleMessages.Clear();
   }

   private void RegisterConsoleMessage(object? _, IConsoleMessage e)
   {
      _consoleMessages.Add(e);
   }
}