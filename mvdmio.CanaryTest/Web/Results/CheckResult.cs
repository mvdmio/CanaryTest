namespace mvdmio.CanaryTest.Web.Results;

public class CheckResult
{
    private List<CheckError> _errors;

    public required Uri Url { get; init; }

    public bool IsSuccess => Errors.Any();
    public IEnumerable<CheckError> Errors => _errors;
    public CheckError? Error => _errors.FirstOrDefault();

    public CheckResult()
    {
        _errors = new List<CheckError>();
    }

    public void AddError(CheckError error)
    {
        _errors.Add(error);
    }

    public void AddErrors(IEnumerable<CheckError> errors)
    {
        _errors.AddRange(errors);
    }

    public void AddConsoleError(string message)
    {
        _errors.Add(new CheckError
        {
            Type = "Console",
            Message = message
        });
    }

    public void AddConsoleErrors(IEnumerable<string> messages)
    {
        _errors.AddRange(messages.Select(x => new CheckError
        {
            Type = "Console",
            Message = x
        }));
    }
}