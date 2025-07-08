using System.Security.Authentication;
using System.Security.Claims;
using CheeseGrater.Core.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CheeseGrater.Core.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
  private readonly IUser _user;
  private readonly IAuthorizationService _authorizationService;

  public IdentityService(IAuthorizationService authorizationService, IUser user)
  {
    _user = user;
    _authorizationService = authorizationService;
  }

  public string? UserId => _user.UserId;

  public string? UserName => _user.UserName;

  public ClaimsPrincipal? Principal => _user.Principal;

  public Task<bool> AuthorizeAsync(string policyName)
  {
    return AuthorizeAsync(GetPrincipal(), policyName);
  }

  public async Task<bool> AuthorizeAsync(object resource, string policyName)
  {
    var principal = GetPrincipal();
    var result = await _authorizationService.AuthorizeAsync(principal, resource, policyName);

    return result.Succeeded;
  }

  public bool IsInRoleAsync(string role) => Principal?.IsInRole(role) ?? false;

  private async Task<bool> AuthorizeAsync(ClaimsPrincipal principal, string policyName)
  {
    var result = await _authorizationService.AuthorizeAsync(principal, policyName);

    return result.Succeeded;
  }

  private ClaimsPrincipal GetPrincipal()
  {
    var principal =
      _user?.Principal
      ?? throw new AuthenticationException("Couldn't find principal. Please authenticate");
    return principal;
  }
}
