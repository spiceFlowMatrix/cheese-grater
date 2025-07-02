namespace CheeseGrater.Core.Application.Common.Interfaces;

public interface IIdentityService : IUser
{
    Task<bool> AuthorizeAsync(string policyName);

    Task<bool> AuthorizeAsync(object resource, string policyName);

    bool IsInRoleAsync(string role);
}
