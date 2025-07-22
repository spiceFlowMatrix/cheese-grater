using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;
using CheeseGrater.Core.Domain.Entities;
using Keycloak.AuthServices.Sdk.Protection;
using Keycloak.AuthServices.Sdk.Protection.Models;

namespace CheeseGrater.Application.TodoLists.Commands.CreateTodoList;

[AuthorizeProtectedResource(Resources.TodoResource, $"{Resources.TodoResource}:{Scopes.Create}")]
public record CreateTodoListCommand : IRequest<int>
{
  public string? Title { get; init; }
}

public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, int>
{
  private readonly IApplicationDbContext _context;
  private readonly IKeycloakProtectionClient _resourceClient;
  private readonly IIdentityService _identityService;

  public CreateTodoListCommandHandler(
    IApplicationDbContext context,
    IKeycloakProtectionClient resourceClient,
    IIdentityService identityService
  )
  {
    _context = context;
    _resourceClient = resourceClient;
    _identityService = identityService;
  }

  public async Task<int> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
  {
    var entity = new TodoList();

    entity.Title = request.Title;

    _context.TodoLists.Add(entity);

    await _context.SaveChangesAsync(cancellationToken);

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

    return entity.Id;
  }
}
