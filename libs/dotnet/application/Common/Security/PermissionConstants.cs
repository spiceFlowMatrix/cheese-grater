using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;
using Keycloak.AuthServices.Sdk.Protection.Models;

namespace CheeseGrater.Application.Common.Security;

public static class PermissionConstants
{
  public static readonly ApplicationPermission TodoItemOwner = new()
  {
    Name = nameof(TodoItemOwner),
    Description = "Permit only TodoItem owners to `use` them",
    DecisionStrategy = DecisionStrategy.AFFIRMATIVE,
    ApplyToResourceType = true,
    ResourceType = $"urn:{Resources.TodoResource}:resource:{Resources.TodoResource}",
    Policies = [PolicyConstants.OwnershipPolicy],
  };
  public static readonly ScopedBasedApplicationPermission UseTodos = new()
  {
    Name = nameof(UseTodos),
    Description = "Permit all users to create/read/update todos",
    DecisionStrategy = DecisionStrategy.AFFIRMATIVE,
    Resources = [Resources.TodoResource],
    Policies = [PolicyConstants.RequireUserRolePolicy, PolicyConstants.RequireAdminRolePolicy],
    Scopes = [Scopes.Create, Scopes.Read, Scopes.Edit],
  };
  public static readonly ScopedBasedApplicationPermission DeleteTodos = new()
  {
    Name = nameof(DeleteTodos),
    Description = "Permit admins to delete todos",
    DecisionStrategy = DecisionStrategy.CONSENSUS,
    Resources = [Resources.TodoResource],
    Policies = [PolicyConstants.RequireAdminRolePolicy],
    Scopes = [Scopes.Delete],
  };
  public static readonly ApplicationPermission DeleteTodoItems = new()
  {
    Name = nameof(DeleteTodoItems),
    Description = "Permit admins to delete owned todo items",
    DecisionStrategy = DecisionStrategy.AFFIRMATIVE,
    ApplyToResourceType = true,
    ResourceType = $"urn:{Resources.TodoResource}:resource:{Resources.TodoResource}",
    Policies = [PolicyConstants.RequireAdminRolePolicy],
  };

  public static readonly List<ApplicationPermission> All =
  [
    TodoItemOwner,
    UseTodos,
    DeleteTodos,
    DeleteTodoItems,
  ];
}
