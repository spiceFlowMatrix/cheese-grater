namespace CheeseGrater.Application.Common.Security;

public enum EPolicyType
{
    RealmRole,
    ClientRole,
}

public record Policy
{
    EPolicyType Type { get; }
    string PolicyName { get; }
    string TargetName { get; }

    public Policy(EPolicyType type, string policyName, string targetName)
    {
        Type = type;
        PolicyName = policyName;
        TargetName = targetName;
    }
}

public static class PolicyConstants
{
    public const string MyCustomPolicy = nameof(MyCustomPolicy);

    public static readonly List<Policy> Policies = new List<Policy>
    {
        new Policy(EPolicyType.RealmRole, MyCustomPolicy, "workspaces:read"),
    };
}
