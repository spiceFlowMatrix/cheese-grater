using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Exceptions;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;
using Keycloak.AuthServices.Authorization;

namespace CheeseGrater.Application.TodoLists.Commands.UpdateTodoList;

[AuthorizeProtectedResource(Resources.TodoResource, $"{Resources.TodoResource}:{Scopes.Edit}")]
public record UpdateTodoListCommand : IRequest
{
  public int Id { get; init; }

  public string? Title { get; init; }
}

public class UpdateTodoListCommandHandler : IRequestHandler<UpdateTodoListCommand>
{
  private readonly IApplicationDbContext _context;
  private readonly IIdentityService _identityService;

  public UpdateTodoListCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService
  )
  {
    _context = context;
    _identityService = identityService;
  }

  public async Task Handle(UpdateTodoListCommand request, CancellationToken cancellationToken)
  {
    var authorized = await _identityService.AuthorizeAsync(
      ProtectedResourcePolicy.From(
        Resources.TodoResourceItem,
        request.Id.ToString(),
        $"{Resources.TodoResourceItem}:{Scopes.Edit}"
      )
    );

    if (!authorized)
    {
      throw new ForbiddenAccessException();
    }

    var entity = await _context.TodoLists.FindAsync(new object[] { request.Id }, cancellationToken);

    Guard.Against.NotFound(request.Id, entity);

    entity.Title = request.Title;

    await _context.SaveChangesAsync(cancellationToken);
  }
}
