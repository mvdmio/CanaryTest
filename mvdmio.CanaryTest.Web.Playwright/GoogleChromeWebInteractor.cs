using System.Diagnostics.CodeAnalysis;
using Microsoft.Playwright;
using mvdmio.CanaryTest.Web.Interfaces;

namespace mvdmio.CanaryTest.Web.Playwright;

public sealed class GoogleChromeWebInteractor : IWebInteractor
{
    private IPage? _page;

    [MemberNotNull(nameof(_page))]
    public async Task Initialize()
    {
        if (_page is not null)
            return;
        
        var playwritght = await Microsoft.Playwright.Playwright.CreateAsync();
        var browser = await playwritght.Chromium.LaunchAsync(new BrowserTypeLaunchOptions {
            Channel = "chrome",
            Headless = false
        });
        
        _page = await browser.NewPageAsync();
    }
    
    public async Task Open(Uri url)
    {
        await Initialize();
        
        await _page.GotoAsync(url.AbsoluteUri);

        if (!_page.Url.Equals(url.AbsoluteUri, StringComparison.InvariantCultureIgnoreCase))
            throw new InvalidOperationException($"Navigating to {url} resulted in redirects to another page. Use {nameof(OpenAndFollowRedirects)} instead of {nameof(Open)} if this is intended.");
    }

    public async Task OpenAndFollowRedirects(Uri url)
    {
        await Initialize();
        
        await _page.GotoAsync(url.AbsoluteUri);
    }

    public async Task EnterText(string elementId, string value)
    {
        await Initialize();
        
        await _page.Locator($"id={elementId}").FillAsync(value);
    }

    public async Task Click(string elementId)
    {
        await Initialize();
        await _page.Locator($"id={elementId}").ClickAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if(_page is not null)
            await _page.Context.DisposeAsync();
    }
}