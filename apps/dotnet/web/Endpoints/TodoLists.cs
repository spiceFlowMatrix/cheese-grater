using CheeseGrater.Application.TodoLists.Commands.CreateTodoList;
using CheeseGrater.Application.TodoLists.Commands.DeleteTodoList;
using CheeseGrater.Application.TodoLists.Commands.UpdateTodoList;
using CheeseGrater.Application.TodoLists.Queries.GetTodos;
using CheeseGrater.Application.TodoLists.Queries.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CheeseGrater.Web.Endpoints;

public class TodoLists : EndpointGroupBase
{
  public override void Map(WebApplication app)
  {
    app.MapGroup(this)
      .RequireAuthorization()
      .MapGet(GetTodoLists)
      .MapPost(CreateTodoList)
      .MapPut(UpdateTodoList, "{id}")
      .MapDelete(DeleteTodoList, "{id}");
  }

  public async Task<Ok<TodosVm>> GetTodoLists(ISender sender)
  {
    var vm = await sender.Send(new GetTodosQuery());

    return TypedResults.Ok(vm);
  }

  public async Task<Created<TodoListDto>> CreateTodoList(
    ISender sender,
    CreateTodoListCommand command
  )
  {
    var dto = await sender.Send(command);

    return TypedResults.Created($"/{nameof(TodoLists)}/{dto.Id}", dto);
  }

  public async Task<Results<Ok<TodoListDto>, BadRequest>> UpdateTodoList(
    ISender sender,
    int id,
    UpdateTodoListCommand command
  )
  {
    if (id != command.Id)
      return TypedResults.BadRequest();

    var dto = await sender.Send(command);

    return TypedResults.Ok(dto);
  }

  public async Task<NoContent> DeleteTodoList(ISender sender, int id)
  {
    await sender.Send(new DeleteTodoListCommand(id));

    return TypedResults.NoContent();
  }
}
