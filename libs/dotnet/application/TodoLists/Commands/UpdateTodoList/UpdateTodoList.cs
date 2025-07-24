using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Application.TodoLists.Queries.Models;
using CheeseGrater.Core.Application.Common.Exceptions;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;
using Keycloak.AuthServices.Authorization;

namespace CheeseGrater.Application.TodoLists.Commands.UpdateTodoList;

[AuthorizeProtectedResource(Resources.TodoResource, $"{Resources.TodoResource}:{Scopes.Edit}")]
public record UpdateTodoListCommand : IRequest<TodoListDto>
{
  public int Id { get; init; }

  public string? Title { get; init; }
}

public class UpdateTodoListCommandHandler : IRequestHandler<UpdateTodoListCommand, TodoListDto>
{
  private readonly IApplicationDbContext _context;
  private readonly IIdentityService _identityService;
  private readonly IMapper _mapper;

  public UpdateTodoListCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService,
    IMapper mapper
  )
  {
    _context = context;
    _identityService = identityService;
    _mapper = mapper;
  }

  public async Task<TodoListDto> Handle(
    UpdateTodoListCommand request,
    CancellationToken cancellationToken
  )
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

    var entity = await _context.TodoLists.FindAsync([request.Id], cancellationToken);

    Guard.Against.NotFound(request.Id, entity);

    entity.Title = request.Title;

    await _context.SaveChangesAsync(cancellationToken);

    return _mapper.Map<TodoListDto>(entity);
  }
}
