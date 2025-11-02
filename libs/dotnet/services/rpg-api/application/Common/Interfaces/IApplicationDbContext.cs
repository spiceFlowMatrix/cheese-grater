using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Domain.Entities;
using CheeseGrater.RpgApi.Domain.Entities;

namespace CheeseGrater.RpgApi.Application.Common.Interfaces;

public interface IApplicationDbContext : IBaseDbContext
{
  DbSet<Player> Players { get; }
  DbSet<Character> Characters { get; }
  // DbSet<TodoList> TodoLists { get; }

  // DbSet<TodoItem> TodoItems { get; }
}
