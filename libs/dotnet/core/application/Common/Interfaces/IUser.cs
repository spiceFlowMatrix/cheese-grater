using System.Security.Claims;

namespace CheeseGrater.Core.Application.Common.Interfaces;

public interface IUser
{
    string? UserId { get; }

    string? UserName { get; }

    ClaimsPrincipal? Principal { get; }
}
