using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;

namespace CheeseGrater.Application.Common.Security;

public static class PolicyConstants
{
  public const string OwnershipPolicy = nameof(OwnershipPolicy);
  public const string RequireUserRolePolicy = nameof(RequireUserRolePolicy);
  public const string RequireAdminRolePolicy = nameof(RequireAdminRolePolicy);

  public static readonly List<ApplicationPolicy> All =
  [
    new ApplicationPolicy(
      EPolicyType.Owner,
      OwnershipPolicy,
      $"urn:{Resources.TodoResource}:resource:{Resources.TodoResource}"
    ),
    new ApplicationPolicy(
      EPolicyType.Role,
      RequireUserRolePolicy,
      Resources.TodoResource,
      [Roles.User]
    ),
    new ApplicationPolicy(
      EPolicyType.Role,
      RequireAdminRolePolicy,
      Resources.TodoResource,
      [Roles.Administrator]
    ),
  ];
}
