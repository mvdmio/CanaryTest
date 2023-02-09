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
    webCrawler = new WebCrawler(new WebCrawlerOptions {
       Uris = new[] { new Uri("https://apps7.jewel.eu/inzameling-acceptatie") },
       IgnoreList = new [] { "Logout" }
    });
    await webCrawler.Initialize(
        async interactor => {
            await interactor.OpenAndFollowRedirects(new Uri("https://apps7.jewel.eu/inzameling-acceptatie"));
            var usernameInput = await interactor.GetElementById("txtUserName");
            var passwordInput = await interactor.GetElementById("txtPassword");
            var loginButton = await interactor.GetElementById("btnLogin");

            await usernameInput.EnterText(userSecrets.Username);
            await passwordInput.EnterText(userSecrets.Password);
            await loginButton.Click(); // Redirects to the subscription picker page

            var pickSubscriptionButton = await interactor.GetElementById("Button1");
            await pickSubscriptionButton.Click(); // Picks the first subscription in the list
        }
    );
    Console.WriteLine("Webcrawler initialized");

    Console.WriteLine("Running webcrawler");
    await webCrawler.Run();    
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
