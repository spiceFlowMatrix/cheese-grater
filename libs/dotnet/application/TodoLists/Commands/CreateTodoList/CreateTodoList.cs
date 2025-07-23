using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Application.TodoLists.Queries.Models;
using CheeseGrater.Core.Application.Common.Exceptions;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;
using CheeseGrater.Core.Domain.Entities;
using Keycloak.AuthServices.Sdk.Protection;
using Keycloak.AuthServices.Sdk.Protection.Models;

namespace CheeseGrater.Application.TodoLists.Commands.CreateTodoList;

[AuthorizeProtectedResource(Resources.TodoResource, $"{Resources.TodoResource}:{Scopes.Create}")]
public record CreateTodoListCommand : IRequest<TodoListDto>
{
  public string? Title { get; init; }
}

public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, TodoListDto>
{
  private readonly IApplicationDbContext _context;
  private readonly IKeycloakProtectionClient _resourceClient;
  private readonly IIdentityService _identityService;
  private readonly IMapper _mapper;

  public CreateTodoListCommandHandler(
    IApplicationDbContext context,
    IKeycloakProtectionClient resourceClient,
    IIdentityService identityService,
    IMapper mapper
  )
  {
    _context = context;
    _resourceClient = resourceClient;
    _identityService = identityService;
    _mapper = mapper;
  }

  public async Task<TodoListDto> Handle(
    CreateTodoListCommand request,
    CancellationToken cancellationToken
  )
  {
    var entity = new TodoList();

    entity.Title = request.Title;

    await _context.SaveChangesAsync(cancellationToken);

    try
    {
      var userId = _identityService?.UserId ?? throw new InvalidOperationException();
      await _resourceClient.CreateResourceAsync(
        "Test",
        new Resource(
          $"{Resources.TodoResourceItem}/{entity.Id}",
          [.. Scopes.All.Select((scope) => $"{Resources.TodoResourceItem}:{scope}")]
        )
        {
          Attributes = { [userId] = "Owner" },
          Type = $"urn:{Resources.TodoResourceItem}-authz:resource:{Resources.TodoResourceItem}",
          OwnerManagedAccess = true,
          Owner = userId,
        },
        cancellationToken
      );

      return _mapper.Map<TodoListDto>(entity);
    }
    catch (Exception ex)
    {
      _context.TodoLists.Remove(entity);

      await _context.SaveChangesAsync(cancellationToken);

      throw new SubprocessFailureException(
        subprocessName: "AuthResourceCreation",
        message: "Failed to create authorization resource for TodoList",
        innerException: ex,
        context: $"{Resources.TodoResourceItem}/{entity.Id}"
      );
    }
  }
}
