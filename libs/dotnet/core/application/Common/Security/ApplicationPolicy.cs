namespace CheeseGrater.Core.Application.Common.Security;

public enum EPolicyType
{
  Role,
  Owner,
}

public record ApplicationPolicy
{
  public EPolicyType Type { get; }
  public string PolicyName { get; }
  public string TargetResource { get; }
  public List<string>? Roles { get; }

  public ApplicationPolicy(
    EPolicyType type,
    string policyName,
    string targetResourceName,
    List<string>? roles = null
  )
  {
    Type = type;
    PolicyName = policyName;
    TargetResource = targetResourceName;
    Roles = roles;
  }
}
