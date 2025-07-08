namespace CheeseGrater.Core.Domain.Constants;

public abstract class Roles
{
  public const string User = nameof(User);
  public const string Administrator = nameof(Administrator);

  public static readonly string[] All = [User, Administrator];
}
