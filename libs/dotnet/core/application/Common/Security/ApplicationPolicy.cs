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
  public List<string>? Roles { get; }

  public ApplicationPolicy(EPolicyType type, string policyName, List<string>? roles = null)
  {
    Type = type;
    PolicyName = policyName;
    Roles = roles;
  }
}
