using mvdmio.CanaryTest.Web.Interfaces;

namespace mvdmio.CanaryTest.Web;

public static class WebInteractorFactory
{
    private static Func<Task<IWebInteractor>>? _asyncBuilderDelegate;
    
    public static void SetBuilder(Func<Task<IWebInteractor>> asyncBuilderDelegate)
    {
        _asyncBuilderDelegate = asyncBuilderDelegate;
    }
    
    public static async Task<IWebInteractor> Create()
    {
        if (_asyncBuilderDelegate is null)
            throw new InvalidOperationException("Builder Delegate has not been initialized yet.");

        return await _asyncBuilderDelegate.Invoke();
    }
}