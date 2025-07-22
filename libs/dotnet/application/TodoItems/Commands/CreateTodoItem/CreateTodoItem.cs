using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Exceptions;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;
using CheeseGrater.Core.Domain.Entities;
using CheeseGrater.Core.Domain.Events;
using Keycloak.AuthServices.Authorization;

namespace CheeseGrater.Application.TodoItems.Commands.CreateTodoItem;

[AuthorizeProtectedResource(Resources.TodoResource, $"{Resources.TodoResource}:{Scopes.Create}")]
public record CreateTodoItemCommand : IRequest<int>
{
  public int ListId { get; init; }

  public string? Title { get; init; }
}

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, int>
{
  private readonly IApplicationDbContext _context;
  private readonly IIdentityService _identityService;

  public CreateTodoItemCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService
  )
  {
    _context = context;
    _identityService = identityService;
  }

  public async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
  {
    var authorized = await _identityService.AuthorizeAsync(
      ProtectedResourcePolicy.From(
        Resources.TodoResourceItem,
        request.ListId.ToString(),
        $"{Resources.TodoResourceItem}:{Scopes.Read}"
      )
    );

    if (!authorized)
    {
      throw new ForbiddenAccessException();
    }

    var entity = new TodoItem
    {
      ListId = request.ListId,
      Title = request.Title,
      Done = false,
    };

    entity.AddDomainEvent(new TodoItemCreatedEvent(entity));

    _context.TodoItems.Add(entity);

    await _context.SaveChangesAsync(cancellationToken);

    return entity.Id;
  }
}
