namespace mvdmio.CanaryTest.Web.Interfaces;

public interface IWebInteractor : IAsyncDisposable
{
    Task Open(Uri url);
    Task OpenAndFollowRedirects(Uri url);
   
    Task<IWebElement> GetElementById(string elementId);
    Task<IEnumerable<IWebElement>> GetElements(string elementName);

    Task<IEnumerable<string>> GetConsoleErrors();
    Task ClearConsole();
}

public interface IWebElement
{
   Task EnterText(string value);
   Task Click();

   Task<string?> GetAttribute(string name);
}