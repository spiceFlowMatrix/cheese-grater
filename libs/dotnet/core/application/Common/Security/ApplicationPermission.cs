namespace CheeseGrater.Core.Application.Common.Security;

public enum EDecisionStrategy
{
  Affirmative,
  Unanimous,
  Consensus,
}

public record ApplicationPermission
{
  public string Name { get; protected set; }
  public string? Description { get; protected set; }
  public bool ApplyToResourceType { get; protected set; } = false;
  public List<string>? Resources { get; protected set; }
  public string? ResourceType { get; protected set; }
  public List<string> Policies { get; protected set; } = [];
}

public record ScopedBasedApplicationPermission : ApplicationPermission
{
  public List<string> Scopes { get; protected set; } = [];
}
