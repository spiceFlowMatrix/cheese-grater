using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;

namespace CheeseGrater.Application.Common.Security;

public static class PolicyConstants
{
  public const string Ownership = nameof(Ownership);
  public const string RequireUserRole = nameof(RequireUserRole);
  public const string RequireAdminRole = nameof(RequireAdminRole);
  public static readonly ApplicationPolicy OwnershipPolicy = new ApplicationPolicy(
    EPolicyType.Owner,
    Ownership
  );
  public static readonly ApplicationPolicy RequireUserRolePolicy = new ApplicationPolicy(
    EPolicyType.Role,
    RequireUserRole,
    [Roles.User]
  );
  public static readonly ApplicationPolicy RequireAdminRolePolicy = new ApplicationPolicy(
    EPolicyType.Role,
    RequireAdminRole,
    [Roles.Administrator]
  );

  public static readonly List<ApplicationPolicy> All =
  [
    OwnershipPolicy,
    RequireUserRolePolicy,
    RequireAdminRolePolicy,
  ];
}
