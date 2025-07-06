namespace CheeseGrater.Core.Application.Common.Security;

public enum EPolicyTargetType
{
    Resource,
    ResourceType
}

public enum EPolicyType
{
    Role,
    Owner
}

public record ApplicationPolicy
{
    public EPolicyType Type { get; }
    public string PolicyName { get; }
    public EPolicyTargetType TargetType { get; }
    public string TargetResource { get; }
    public List<string>? Roles { get; }

    public ApplicationPolicy(EPolicyType type, string policyName, EPolicyTargetType targetType, string targetResourceName, List<string>? roles = null)
    {
        Type = type;
        PolicyName = policyName;
        TargetType = targetType;
        TargetResource = targetResourceName;
        Roles = roles;
    }
}