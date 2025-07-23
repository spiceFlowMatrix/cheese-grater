using CheeseGrater.Application.TodoLists.Queries.Models;
using CheeseGrater.Core.Application.Common.Models;

namespace CheeseGrater.Application.TodoLists.Queries.GetTodos;

public class TodosVm
{
  public IReadOnlyCollection<LookupDto> PriorityLevels { get; init; } = Array.Empty<LookupDto>();

  public IReadOnlyCollection<TodoListDto> Lists { get; init; } = Array.Empty<TodoListDto>();
}
