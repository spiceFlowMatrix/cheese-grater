using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Exceptions;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;
using CheeseGrater.Core.Domain.Enums;
using Keycloak.AuthServices.Authorization;

namespace CheeseGrater.Application.TodoItems.Commands.UpdateTodoItemDetail;

[AuthorizeProtectedResource(Resources.TodoResource, $"{Resources.TodoResource}:{Scopes.Edit}")]
public record UpdateTodoItemDetailCommand : IRequest
{
  public int Id { get; init; }

  public int ListId { get; init; }

  public PriorityLevel Priority { get; init; }

  public string? Note { get; init; }
}

public class UpdateTodoItemDetailCommandHandler : IRequestHandler<UpdateTodoItemDetailCommand>
{
  private readonly IApplicationDbContext _context;
  private readonly IIdentityService _identityService;

  public UpdateTodoItemDetailCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService
  )
  {
    _context = context;
    _identityService = identityService;
  }

  public async Task Handle(UpdateTodoItemDetailCommand request, CancellationToken cancellationToken)
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

    entity.ListId = request.ListId;
    entity.Priority = request.Priority;
    entity.Note = request.Note;

    await _context.SaveChangesAsync(cancellationToken);
  }
}
