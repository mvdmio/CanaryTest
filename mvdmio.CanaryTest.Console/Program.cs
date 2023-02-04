using mvdmio.CanaryTest;
using mvdmio.CanaryTest.Web;
using mvdmio.CanaryTest.Web.Playwright.Setup;

WebCrawler? webCrawler = null;
try
{
    CanaryTest.Setup.WebCrawlWithPlaywright();

    Console.WriteLine("Initializing webcrawler");
    webCrawler = new WebCrawler("https://inzameling.jewel.eu");
    await webCrawler.Initialize(
        async interactor => {
            await interactor.OpenAndFollowRedirects(new Uri("https://inzameling.jewel.eu"));
            await interactor.EnterText("txtUserName", "mvdmeer@jewelsoftware.com");
            await interactor.EnterText("txtPassword", @".\private$\logmessage");
            await interactor.Click("btnLogin");
            await interactor.Click("Button1");
        }
    );
    Console.WriteLine("Webcrawler initialized");

    Console.WriteLine("Running webcrawler");
    webCrawler.Run();    
}
catch (Exception e)
{
    Console.WriteLine("An unexpected error ocurred.");
    Console.WriteLine(e.ToString());
}
finally
{
    Console.WriteLine("Press any key to exit.");
    Console.ReadKey();
    
    if (webCrawler is not null)
        await webCrawler.DisposeAsync();
}
