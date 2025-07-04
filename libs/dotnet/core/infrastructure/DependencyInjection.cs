using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Infrastructure.Data.Interceptors;
using CheeseGrater.Core.Infrastructure.Identity;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Authorization.Claims;
using Keycloak.AuthServices.Common;
using Keycloak.AuthServices.Sdk;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddCoreInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();


        builder.Services.AddAuthorization(o =>
        {
            o.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        }).AddKeycloakAuthorization()
        .AddAuthorizationServer(builder.Configuration);

        builder.Services.AddSingleton<IAuthorizationPolicyProvider, ProtectedResourcePolicyProvider>();

        var keycloakOptions = builder.Configuration.GetKeycloakOptions<KeycloakProtectionClientOptions>()!;
        const string tokenClientName = "KeycloakProtectionClient";

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddClientCredentialsTokenManagement()
            .AddClient(
                tokenClientName,
                client =>
                {
                    client.ClientId = keycloakOptions.Resource;
                    client.ClientSecret = keycloakOptions.Credentials.Secret;
                    client.TokenEndpoint = keycloakOptions.KeycloakTokenEndpoint;
                }
            );

        builder.Services.AddKeycloakProtectionHttpClient(builder.Configuration)
            .AddClientCredentialsTokenHandler(tokenClientName);
    }
}
