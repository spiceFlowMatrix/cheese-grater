using CheeseGrater.Core.Domain.Constants;
using CheeseGrater.Core.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CheeseGrater.RpgApi.Infrastructure.Data;

public static class InitialiserExtensions
{
  public static void AddAsyncSeeding(
    this DbContextOptionsBuilder builder,
    IServiceProvider serviceProvider
  )
  {
    builder.UseAsyncSeeding(
      async (context, _, ct) =>
      {
        var initialiser = serviceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.SeedAsync();
      }
    );
  }

  public static async Task InitialiseDatabaseAsync(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();

    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

    await initialiser.InitialiseAsync();
  }
}

public class ApplicationDbContextInitialiser
{
  private readonly ILogger<ApplicationDbContextInitialiser> _logger;
  private readonly ApplicationDbContext _context;

  public ApplicationDbContextInitialiser(
    ILogger<ApplicationDbContextInitialiser> logger,
    ApplicationDbContext context
  )
  {
    _logger = logger;
    _context = context;
  }

  public async Task InitialiseAsync()
  {
    try
    {
      await _context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while initialising the database.");
      throw;
    }
  }

  public async Task SeedAsync()
  {
    try
    {
      await TrySeedAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while seeding the database.");
      throw;
    }
  }

  public async Task TrySeedAsync() { }
}
