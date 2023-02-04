namespace mvdmio.CanaryTest.Web.Playwright.Setup;

public static class PlaywrightCanaryTestSetup
{
    public static void WebCrawlWithPlaywright(this CanaryTestSetup setup)
    {
        WebInteractorFactory.SetBuilder(() => new GoogleChromeWebInteractor());
    }
}