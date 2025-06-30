using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Domain.Constants;
using CheeseGrater.Infrastructure.Data;
using CheeseGrater.Infrastructure.Data.Interceptors;
using CheeseGrater.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
            if (!IsDesignTime()) options.AddAsyncSeeding(sp);
            options.UseSnakeCaseNamingConvention();
        });


        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services
            .AddDefaultIdentity<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();

        builder.Services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));
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
