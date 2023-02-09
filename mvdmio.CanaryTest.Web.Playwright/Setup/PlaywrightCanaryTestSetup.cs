namespace mvdmio.CanaryTest.Web.Playwright.Setup;

public static class PlaywrightCanaryTestSetup
{
    public static void WebCrawlWithPlaywright(this CanaryTestSetup _)
    {
        WebInteractorFactory.SetBuilder(async () => await GoogleChromeWebInteractor.Build());
    }
}