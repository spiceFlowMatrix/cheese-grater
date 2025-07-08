using CheeseGrater.Core.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CheeseGrater.Core.Infrastructure.Data;

public abstract class BaseDbContext : DbContext, IBaseDbContext
{
  private readonly string _defaultSchema;

  public BaseDbContext(DbContextOptions options, IConfiguration configuration)
    : base(options)
  {
    _defaultSchema = configuration.GetValue<string>("EfCore:DefaultSchema") ?? "public";
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.HasDefaultSchema(_defaultSchema);
    base.OnModelCreating(builder);
  }
}
