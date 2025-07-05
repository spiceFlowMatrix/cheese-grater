using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;
namespace CheeseGrater.Application.Common.Security;

public static class PolicyConstants
{
    public const string OwnershipPolicy = nameof(OwnershipPolicy);
    public const string RequireUserRolePolicy = nameof(RequireUserRolePolicy);

    public static readonly List<Policy> Policies =
    [
        new Policy(EPolicyType.Owner, OwnershipPolicy, EPolicyTargetType.ResourceType, $"urn:{Resources.TodoResource}:resource:{Resources.TodoResource}"),
        new Policy(EPolicyType.Role, RequireUserRolePolicy, EPolicyTargetType.Resource, Resources.TodoResource),
    ];
}
