using System.Reflection;
using CheeseGrater.Core.Infrastructure.Data;
using CheeseGrater.RpgApi.Application.Common.Interfaces;
using CheeseGrater.RpgApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CheeseGrater.RpgApi.Infrastructure.Data;

public class ApplicationDbContext : BaseDbContext, IApplicationDbContext
{
  public ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IConfiguration configuration
  )
    : base(options, configuration) { }

  public DbSet<Player> Players => Set<Player>();
  public DbSet<Character> Characters => Set<Character>();

  // public DbSet<TodoList> TodoLists => Set<TodoList>();
  // public DbSet<TodoItem> TodoItems => Set<TodoItem>();

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);
    builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
