using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;

namespace CheeseGrater.Application.Common.Security;

public static class PermissionConstants
{
  public static readonly ScopedBasedApplicationPermission TodoItemOwner = new()
  {
    Name = nameof(TodoItemOwner),
    Description = "Permit only TodoItem owners to `use` them",
    DecisionStrategy = EDecisionStrategy.AFFIRMATIVE,
    ResourceType = $"urn:{Resources.TodoResourceItem}-authz:resource:{Resources.TodoResourceItem}",
    Scopes = [.. Scopes.All.Select((scope) => $"{Resources.TodoResourceItem}:{scope}")],
    Policies = [PolicyConstants.OwnershipPolicy, PolicyConstants.RequireAdminRolePolicy],
  };

  public static readonly ScopedBasedApplicationPermission UseTodos = new()
  {
    Name = nameof(UseTodos),
    Description = "Permit all users to create/read/update todos",
    DecisionStrategy = EDecisionStrategy.AFFIRMATIVE,
    Resources = [Resources.TodoResource],
    Policies = [PolicyConstants.RequireUserRolePolicy, PolicyConstants.RequireAdminRolePolicy],
    Scopes =
    [
      $"{Resources.TodoResource}:{Scopes.Create}",
      $"{Resources.TodoResource}:{Scopes.Read}",
      $"{Resources.TodoResource}:{Scopes.Edit}",
      $"{Resources.TodoResource}:{Scopes.List}",
    ],
  };

  public static readonly ScopedBasedApplicationPermission DeleteTodos = new()
  {
    Name = nameof(DeleteTodos),
    Description = "Permit admins to delete todos",
    DecisionStrategy = EDecisionStrategy.CONSENSUS,
    Resources = [Resources.TodoResource],
    Policies = [PolicyConstants.RequireAdminRolePolicy],
    Scopes = [$"{Resources.TodoResource}:{Scopes.Delete}"],
  };

  public static readonly List<ApplicationPermission> All = [TodoItemOwner, UseTodos, DeleteTodos];
}
