using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;
using CheeseGrater.Core.Domain.Entities;
using CheeseGrater.Core.Domain.Events;
using Keycloak.AuthServices.Sdk.Protection;
using Keycloak.AuthServices.Sdk.Protection.Models;

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
  private readonly IKeycloakProtectionClient _resourceClient;
  private readonly IIdentityService _identityService;

  public CreateTodoItemCommandHandler(
    IApplicationDbContext context,
    IKeycloakProtectionClient resourceClient,
    IIdentityService identityService
  )
  {
    _context = context;
    _resourceClient = resourceClient;
    _identityService = identityService;
  }

  public async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
  {
    var entity = new TodoItem
    {
      ListId = request.ListId,
      Title = request.Title,
      Done = false,
    };

    entity.AddDomainEvent(new TodoItemCreatedEvent(entity));

    _context.TodoItems.Add(entity);

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
