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

public record Policy
{
    public EPolicyType Type { get; }
    public string PolicyName { get; }
    public EPolicyTargetType TargetType { get; }
    public string TargetResource { get; }

    public Policy(EPolicyType type, string policyName, EPolicyTargetType targetType, string targetResourceName)
    {
        Type = type;
        PolicyName = policyName;
        TargetType = targetType;
        TargetResource = targetResourceName;
    }
}