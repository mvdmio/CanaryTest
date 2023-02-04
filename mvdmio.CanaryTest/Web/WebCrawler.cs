using mvdmio.CanaryTest.Web.Interfaces;

namespace mvdmio.CanaryTest.Web;

public class WebCrawler : IAsyncDisposable
{
    private readonly Uri _startUrl;
    private readonly WebCrawlerOptions _options;

    private IWebInteractor? _webInteractor;

    public WebCrawler(string startUrl) : this(new Uri(startUrl)) { }
    public WebCrawler(Uri startUrl) : this(startUrl, new WebCrawlerOptions()) { }
    public WebCrawler(string startUrl, WebCrawlerOptions options) : this(new Uri(startUrl), options) { }

    public WebCrawler(Uri startUrl, WebCrawlerOptions options)
    {
        _startUrl = startUrl;
        _options = options;
    }

    /// <summary>
    /// Initializes the webcrawler.
    /// You can use this method to setup the state of the webcrawler before the run starts.
    /// For instance, to log in with a user account.
    /// </summary>
    public async Task Initialize(Func<IWebInteractor, Task> initializationDelegate)
    {
        _webInteractor ??= WebInteractorFactory.Create();
        await initializationDelegate.Invoke(_webInteractor);
    }
    
    public void Run()
    {
        _webInteractor ??= WebInteractorFactory.Create();

        _webInteractor.Open(_startUrl);
        
        //TODO: Start crawling
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        
        if(_webInteractor is not null)
         await _webInteractor.DisposeAsync();
    }
}