using Keycloak.AuthServices.Sdk.Protection.Models;

namespace CheeseGrater.Core.Application.Common.Security;

public record ApplicationPermission
{
  public required string Name { get; init; }
  public string? Description { get; init; }
  public DecisionStrategy DecisionStrategy { get; init; } = DecisionStrategy.UNANIMOUS;
  public bool ApplyToResourceType { get; init; } = false;
  public List<string>? Resources { get; init; }
  public string? ResourceType { get; init; }
  public List<ApplicationPolicy> Policies { get; init; } = [];
}

public record ScopedBasedApplicationPermission : ApplicationPermission
{
  public List<string> Scopes { get; init; } = [];
}
