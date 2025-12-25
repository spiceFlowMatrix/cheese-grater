using CheeseGrater.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CheeseGrater.Web.Endpoints;

public class IdentityFrontend : EndpointGroupBase
{
  public override void Map(WebApplication app)
  {
    app.MapGroup("/api/identity")
      .WithTags("Identity")
      .AllowAnonymous()
      .MapGet("/spa-config", GetSpaConfig);
  }

  private static IResult GetSpaConfig(
    IConfiguration configuration,
    IOptions<SpaClientOptions> spaClientOptions
  )
  {
    var spaOptions = spaClientOptions.Value ?? new SpaClientOptions();

    var authServerUrl = EnsureTrailingSlash(
      configuration["Keycloak:auth-server-url"] ?? string.Empty
    );
    var realm = configuration["Keycloak:realm"] ?? string.Empty;
    var rootUrl = EnsureTrailingSlash(spaOptions.RootUrl);

    var response = new SpaAuthConfigDto
    {
      AuthServerUrl = authServerUrl,
      Realm = realm,
      ClientId = spaOptions.ClientId,
      RedirectUri = rootUrl,
      LogoutRedirectUri = rootUrl,
      RequireHttps = spaOptions.RequireHttps,
    };

    return Results.Ok(response);
  }

  private static string EnsureTrailingSlash(string value)
  {
    var trimmed = value?.Trim() ?? string.Empty;
    return trimmed.EndsWith("/") ? trimmed : $"{trimmed}/";
  }
}
