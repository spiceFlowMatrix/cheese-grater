using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Entities;
using CheeseGrater.Core.Domain.Events;
using Keycloak.AuthServices.Sdk.Protection;
using Keycloak.AuthServices.Sdk.Protection.Models;

namespace CheeseGrater.Application.TodoItems.Commands.CreateTodoItem;

[AuthorizeProtectedResource("workspaces", "workspace:read")]
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

    public CreateTodoItemCommandHandler(IApplicationDbContext context, IKeycloakProtectionClient resourceClient, IIdentityService identityService)
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
            Done = false
        };

        entity.AddDomainEvent(new TodoItemCreatedEvent(entity));

        _context.TodoItems.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        var userName = _identityService?.UserName ?? throw new InvalidOperationException();
        await _resourceClient.CreateResourceAsync(
            "master",
            new Resource(
                $"workspaces/{entity.Id}",
                ["workspaces:read", "workspaces:delete"]
            )
            {
                Attributes = { [userName] = "Owner" },
                Type = "urn:workspace-authz:resource:workspaces",
            },
            cancellationToken
        );

        return entity.Id;
    }
}
