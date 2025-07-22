using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Exceptions;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;
using Keycloak.AuthServices.Authorization;

namespace CheeseGrater.Application.TodoItems.Commands.DeleteTodoItem;

[AuthorizeProtectedResource(Resources.TodoResource, $"{Resources.TodoResource}:{Scopes.Delete}")]
public record DeleteTodoItemCommand(int Id) : IRequest;

public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand>
{
  private readonly IApplicationDbContext _context;
  private readonly IIdentityService _identityService;

  public DeleteTodoItemCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService
  )
  {
    _context = context;
    _identityService = identityService;
  }

  public async Task Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
  {
    var entity = await _context.TodoItems.FindAsync(new object[] { request.Id }, cancellationToken);

    Guard.Against.NotFound(request.Id, entity);

    var authorized = await _identityService.AuthorizeAsync(
      ProtectedResourcePolicy.From(
        Resources.TodoResourceItem,
        entity.ListId.ToString(),
        $"{Resources.TodoResourceItem}:{Scopes.Read}"
      )
    );

    if (!authorized)
    {
      throw new ForbiddenAccessException();
    }

    _context.TodoItems.Remove(entity);

    entity.AddDomainEvent(new TodoItemDeletedEvent(entity));

    await _context.SaveChangesAsync(cancellationToken);
  }
}
