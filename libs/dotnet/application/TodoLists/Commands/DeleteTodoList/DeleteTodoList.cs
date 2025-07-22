using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Exceptions;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;
using Keycloak.AuthServices.Authorization;

namespace CheeseGrater.Application.TodoLists.Commands.DeleteTodoList;

[AuthorizeProtectedResource(Resources.TodoResource, $"{Resources.TodoResource}:{Scopes.Delete}")]
public record DeleteTodoListCommand(int Id) : IRequest;

public class DeleteTodoListCommandHandler : IRequestHandler<DeleteTodoListCommand>
{
  private readonly IApplicationDbContext _context;
  private readonly IIdentityService _identityService;

  public DeleteTodoListCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService
  )
  {
    _context = context;
    _identityService = identityService;
  }

  public async Task Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
  {
    var authorized = await _identityService.AuthorizeAsync(
      ProtectedResourcePolicy.From(
        Resources.TodoResourceItem,
        request.Id.ToString(),
        $"{Resources.TodoResourceItem}:{Scopes.Delete}"
      )
    );

    if (!authorized)
    {
      throw new ForbiddenAccessException();
    }

    var entity = await _context
      .TodoLists.Where(l => l.Id == request.Id)
      .SingleOrDefaultAsync(cancellationToken);

    Guard.Against.NotFound(request.Id, entity);

    _context.TodoLists.Remove(entity);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
