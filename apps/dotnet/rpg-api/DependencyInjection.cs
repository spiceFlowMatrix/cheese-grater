using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Azure.Identity;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.RpgApi.Infrastructure.Data;
using CheeseGrater.RpgApi.Services;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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

    builder.Services.AddSwaggerGen(options =>
    {
      options.SwaggerDoc(
        "signalr-v1",
        new OpenApiInfo { Title = "CheeseGrater SignalR API", Version = "v1" }
      );
      options.AddSignalRSwaggerGen();

      // If using XML comments, include the XML file
      var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
      options.IncludeXmlComments(xmlPath);
    });

    builder.Services.AddOpenApiDocument(
      (configure, sp) =>
      {
        configure.Title = "CheeseGrater API";
      }
    );

    builder
      .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      // .AddJwtBearer(
      //   (opts) =>
      //   {
      //     opts.Events = new JwtBearerEvents
      //     {
      //       OnMessageReceived = context =>
      //       {
      //         var accessToken = context.Request.Query["access_token"].FirstOrDefault();
      //         var path = context.HttpContext.Request.Path;
      //         if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/game"))
      //         {
      //           context.Token = accessToken;
      //         }
      //         return Task.CompletedTask;
      //       },
      //     };

      //     // Optional: make name / role claim mapping explicit if Keycloak uses different claim names
      //     opts.TokenValidationParameters = new TokenValidationParameters
      //     {
      //       NameClaimType = JwtRegisteredClaimNames.PreferredUsername, // map 'sub' to Identity.Name if you like
      //       RoleClaimType = "roles", // or "realm_access.roles" depending on Keycloak config
      //     };
      //   }
      // )
      .AddKeycloakWebApi(builder.Configuration);

    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

    builder.Services.AddSignalR().AddJsonProtocol();
  }
}
