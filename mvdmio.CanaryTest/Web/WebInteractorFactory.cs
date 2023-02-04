using mvdmio.CanaryTest.Web.Interfaces;

namespace mvdmio.CanaryTest.Web;

public static class WebInteractorFactory
{
    private static Func<IWebInteractor>? _builderDelegate;
    
    public static void SetBuilder(Func<IWebInteractor> builderDelegate)
    {
        _builderDelegate = builderDelegate;
    }
    
    public static IWebInteractor Create()
    {
        if (_builderDelegate is null)
            throw new InvalidOperationException("Builder Delegate has not been initialized yet.");

        return _builderDelegate.Invoke();
    }
}