using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using CheeseGrater.Infrastructure.Identity;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Sdk;
using Keycloak.AuthServices.Common;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.AddCoreInfrastructureServices();
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        // Read EF Core options from config
        var efCoreSection = builder.Configuration.GetSection("EfCore");
        var migrationsTable = efCoreSection["MigrationHistoryTable"] ?? "__efmigrationshistory";
        var migrationsSchema = efCoreSection["MigrationHistorySchema"] ?? "public";

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable(migrationsTable, migrationsSchema);
            });
            if (!IsDesignTime()) options.AddAsyncSeeding(sp);
            options.UseSnakeCaseNamingConvention();
        });


        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services.AddAuthorization(o =>
        {
            o.AddPolicy(PolicyConstants.MyCustomPolicy, b =>
            {
                // b.AddRequirements(new DecisionRequirement("workspaces", "workspaces:read"));
                b.RequireProtectedResource("workspaces", "workspaces:read");
            });

            o.AddPolicy(PolicyConstants.CanDeleteAllWorkspaces, b =>
            {
                b.RequireRealmRoles("SuperManager");
            });

            o.AddPolicy(PolicyConstants.AccessManagement, b =>
            {
                b.RequireResourceRoles("Manager");
            });
        });

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

    /// <summary>
    /// Check if the application is running in a design-time context (e.g., dotnet ef)
    /// </summary>
    /// <returns>bool</returns>
    private static bool IsDesignTime()
    {
        var args = Environment.GetCommandLineArgs();
        return args.Any(arg => arg.Contains("ef", StringComparison.OrdinalIgnoreCase));
    }
}
