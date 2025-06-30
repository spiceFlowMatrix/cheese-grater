using Azure.Identity;
using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Infrastructure.Data;
using CheeseGrater.Web.Services;
using Microsoft.AspNetCore.Mvc;


namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<IUser, CurrentUser>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        builder.Services.AddExceptionHandler<CustomExceptionHandler>();

        builder.Services.AddRazorPages();

        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();

        // builder.Services.AddOpenApiDocument((configure, sp) =>
        // {
        //     configure.Title = "CheeseGrater API";

        // });
    }

    // public static void AddKeyVaultIfConfigured(this IHostApplicationBuilder builder)
    // {
    //     var keyVaultUri = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];
    //     if (!string.IsNullOrWhiteSpace(keyVaultUri))
    //     {
    //         builder.Configuration.AddAzureKeyVault(
    //             new Uri(keyVaultUri),
    //             new DefaultAzureCredential());
    //     }
    // }
}
