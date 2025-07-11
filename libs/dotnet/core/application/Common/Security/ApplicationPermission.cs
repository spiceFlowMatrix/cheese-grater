namespace CheeseGrater.Core.Application.Common.Security;

public enum EDecisionStrategy
{
  AFFIRMATIVE,
  UNANIMOUS,
  CONSENSUS,
}

public record ApplicationPermission
{
  public required string Name { get; init; }
  public string? Description { get; init; }
  public EDecisionStrategy DecisionStrategy { get; init; } = EDecisionStrategy.UNANIMOUS;
  public List<string>? Resources { get; init; }
  public string? ResourceType { get; init; }
  public List<ApplicationPolicy> Policies { get; init; } = [];
}

public record ScopedBasedApplicationPermission : ApplicationPermission
{
  public List<string> Scopes { get; init; } = [];
}
