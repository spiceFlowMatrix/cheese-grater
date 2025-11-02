using Azure.Identity;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Infrastructure.Data;
using CheeseGrater.RpgApi.Services;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
  public static void AddWebServices(this IHostApplicationBuilder builder)
  {
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services.AddScoped<IUser, CurrentUser>();

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>();

    builder.Services.AddExceptionHandler<CustomExceptionHandler>();

    // Customise default API behaviour
    builder.Services.Configure<ApiBehaviorOptions>(options =>
      options.SuppressModelStateInvalidFilter = true
    );

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddOpenApiDocument(
      (configure, sp) =>
      {
        configure.Title = "CheeseGrater API";
      }
    );

    builder
      .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddKeycloakWebApi(builder.Configuration);
  }
}
