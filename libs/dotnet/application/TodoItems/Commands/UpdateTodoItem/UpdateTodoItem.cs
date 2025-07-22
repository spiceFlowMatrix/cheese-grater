using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Exceptions;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;
using Keycloak.AuthServices.Authorization;

namespace CheeseGrater.Application.TodoItems.Commands.UpdateTodoItem;

[AuthorizeProtectedResource(Resources.TodoResource, $"{Resources.TodoResource}:{Scopes.Edit}")]
public record UpdateTodoItemCommand : IRequest
{
  public int Id { get; init; }

  public string? Title { get; init; }

  public bool Done { get; init; }
}

public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand>
{
  private readonly IApplicationDbContext _context;
  private readonly IIdentityService _identityService;

  public UpdateTodoItemCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService
  )
  {
    _context = context;
    _identityService = identityService;
  }

  public async Task Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
  {
    var entity = await _context.TodoItems.FindAsync([request.Id], cancellationToken);

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

    entity.Title = request.Title;
    entity.Done = request.Done;

    await _context.SaveChangesAsync(cancellationToken);
  }
}
