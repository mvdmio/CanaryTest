namespace mvdmio.CanaryTest.Web.Interfaces;

public interface IWebInteractor : IAsyncDisposable
{
    Task Open(Uri url);
    Task OpenAndFollowRedirects(Uri url);
    Task EnterText(string elementId, string value);
    Task Click(string elementId);
}