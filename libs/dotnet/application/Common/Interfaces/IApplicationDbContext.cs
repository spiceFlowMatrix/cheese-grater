using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Domain.Entities;

namespace CheeseGrater.Application.Common.Interfaces;

public interface IApplicationDbContext : IBaseDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }
}
