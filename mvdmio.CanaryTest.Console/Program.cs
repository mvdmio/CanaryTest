using mvdmio.CanaryTest;
using mvdmio.CanaryTest.Console;
using mvdmio.CanaryTest.Web;
using mvdmio.CanaryTest.Web.Playwright.Setup;

WebCrawler? webCrawler = null;
try
{
    var userSecrets = UserSecretsReader.Read<UserSecrets>("CanaryTest");

    if (userSecrets is null)
    {
        Console.WriteLine("Could not find User Secrets. Aborting.");
        return;
    }
    
    CanaryTest.Setup.WebCrawlWithPlaywright();

    Console.WriteLine("Initializing webcrawler");
    webCrawler = new WebCrawler("https://inzameling.jewel.eu");
    await webCrawler.Initialize(
        async interactor => {
            await interactor.OpenAndFollowRedirects(new Uri("https://inzameling.jewel.eu"));
            await interactor.EnterText("txtUserName", userSecrets.Username);
            await interactor.EnterText("txtPassword", userSecrets.Password);
            await interactor.Click("btnLogin"); // Redirects to the subscription picker page
            await interactor.Click("Button1"); // Picks the first subscription in the list
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
