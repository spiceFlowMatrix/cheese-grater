namespace CheeseGrater.Application.Common.Security;

public sealed class SpaAuthConfigDto
{
  public required string AuthServerUrl { get; init; }
  public required string Realm { get; init; }
  public required string ClientId { get; init; }
  public required string RedirectUri { get; init; }
  public required string LogoutRedirectUri { get; init; }
  public bool RequireHttps { get; init; }
}
