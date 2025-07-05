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
    EPolicyType Type { get; }
    string PolicyName { get; }
    EPolicyTargetType TargetType { get; }
    string TargetResource { get; }

    public Policy(EPolicyType type, string policyName, EPolicyTargetType targetType, string targetResourceName)
    {
        Type = type;
        PolicyName = policyName;
        TargetType = targetType;
        TargetResource = targetResourceName;
    }
}