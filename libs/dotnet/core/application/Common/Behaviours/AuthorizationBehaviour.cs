using System.Reflection;
using CheeseGrater.Core.Application.Common.Exceptions;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Security;
using Microsoft.Extensions.Logging;

namespace CheeseGrater.Core.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<TRequest> _logger;

    public AuthorizationBehaviour(IIdentityService identityService, ILogger<TRequest> logger)
    {
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        if (!authorizeAttributes.Any())
        {
            return await next();
        }

        // Must be authenticated user
        if (_identityService.UserId == null)
        {
            throw new UnauthorizedAccessException();
        }

        EnsureAuthorizedRoles(authorizeAttributes);

        await EnsureAuthorizedPolicies(request, authorizeAttributes);

        // User is authorized / authorization not required
        return await next();
    }

    private async Task EnsureAuthorizedPolicies(
        TRequest request,
        IEnumerable<AuthorizeAttribute> authorizeAttributes
    )
    {
        // Policy-based authorization
        var authorizeAttributesWithPolicies = authorizeAttributes
            .Where(a => !string.IsNullOrWhiteSpace(a.Policy))
            .ToList();

        if (!authorizeAttributesWithPolicies.Any())
        {
            return;
        }

        var requiredPolicies = authorizeAttributesWithPolicies.Select(a =>
        {
            if (
                a is AuthorizeProtectedResourceAttribute resourceAttribute
                && request is IRequestWithResourceId requestWithResourceId
            )
            {
                resourceAttribute.ResourceId = requestWithResourceId.ResourceId;
            }
            return a.Policy;
        });

        foreach (var policy in requiredPolicies)
        {
            var authorized = await _identityService.AuthorizeAsync(
                _identityService.Principal!,
                policy!
            );

            if (authorized)
            {
                continue;
            }

#pragma warning disable CA1848 // Use the LoggerMessage delegates
            _logger.LogDebug("Failed policy authorization {Policy}", policy);
#pragma warning restore CA1848 // Use the LoggerMessage delegates
            throw new ForbiddenAccessException();
        }
    }

    private void EnsureAuthorizedRoles(IEnumerable<AuthorizeAttribute> authorizeAttributes)
    {
        // Role-based authorization
        var authorizeAttributesWithRoles = authorizeAttributes
            .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
            .ToList();

        if (!authorizeAttributesWithRoles.Any())
        {
            return;
        }

        var requiredRoles = authorizeAttributesWithRoles
            .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
            .Select(a => a.Roles!.Split(','));

        if (
            requiredRoles
                .Select(roles => roles.Any(role => _identityService.IsInRoleAsync(role.Trim())))
                .Any(authorized => !authorized)
        )
        {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
            _logger.LogDebug("Failed role authorization");
#pragma warning restore CA1848 // Use the LoggerMessage delegates

            throw new ForbiddenAccessException();
        }
    }
}
