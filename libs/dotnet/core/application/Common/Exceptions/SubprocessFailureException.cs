namespace CheeseGrater.Core.Application.Common.Exceptions;

public class SubprocessFailureException : Exception
{
  public string SubprocessName { get; }
  public string? Context { get; }

  public SubprocessFailureException(string subprocessName, string message, string? context = null)
    : base(message)
  {
    SubprocessName = subprocessName ?? throw new ArgumentNullException(nameof(subprocessName));
    Context = context;
  }

  public SubprocessFailureException(
    string subprocessName,
    string message,
    Exception innerException,
    string? context = null
  )
    : base(message, innerException)
  {
    SubprocessName = subprocessName ?? throw new ArgumentNullException(nameof(subprocessName));
    Context = context;
  }
}
