using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;

namespace CheeseGrater.Application.Common.Security;

public static class PolicyConstants
{
  public static readonly ApplicationPolicy OwnershipPolicy = new ApplicationPolicy(
    EPolicyType.Owner,
    nameof(OwnershipPolicy)
  );
  public static readonly ApplicationPolicy RequireUserRolePolicy = new ApplicationPolicy(
    EPolicyType.Role,
    nameof(RequireUserRolePolicy),
    [Roles.User]
  );
  public static readonly ApplicationPolicy RequireAdminRolePolicy = new ApplicationPolicy(
    EPolicyType.Role,
    nameof(RequireAdminRolePolicy),
    [Roles.Administrator]
  );

  public static readonly List<ApplicationPolicy> All =
  [
    OwnershipPolicy,
    RequireUserRolePolicy,
    RequireAdminRolePolicy,
  ];
}
