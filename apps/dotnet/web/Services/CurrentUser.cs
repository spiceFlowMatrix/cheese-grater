using System.Security.Claims;
using CheeseGrater.Core.Application.Common.Interfaces;

namespace CheeseGrater.Web.Services;

public class CurrentUser : IUser
{
  private readonly IHttpContextAccessor httpContextAccessor;

  public CurrentUser(IHttpContextAccessor httpContextAccessor) =>
    this.httpContextAccessor = httpContextAccessor;

  public string? UserId =>
    this.httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

  public string? UserName =>
    this.httpContextAccessor.HttpContext?.User?.FindFirstValue("preferred_username");

  public ClaimsPrincipal? Principal => this.httpContextAccessor.HttpContext?.User;
}
