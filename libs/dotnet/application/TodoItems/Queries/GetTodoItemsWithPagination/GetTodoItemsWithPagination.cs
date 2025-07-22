using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Application.Common.Security;
using CheeseGrater.Core.Application.Common.Exceptions;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Mappings;
using CheeseGrater.Core.Application.Common.Models;
using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;
using Keycloak.AuthServices.Authorization;

namespace CheeseGrater.Application.TodoItems.Queries.GetTodoItemsWithPagination;

[AuthorizeProtectedResource(Resources.TodoResource, $"{Resources.TodoResource}:{Scopes.List}")]
public record GetTodoItemsWithPaginationQuery : IRequest<PaginatedList<TodoItemBriefDto>>
{
  public int ListId { get; init; }
  public int PageNumber { get; init; } = 1;
  public int PageSize { get; init; } = 10;
}

public class GetTodoItemsWithPaginationQueryHandler
  : IRequestHandler<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemBriefDto>>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;
  private readonly IIdentityService _identityService;

  public GetTodoItemsWithPaginationQueryHandler(
    IApplicationDbContext context,
    IMapper mapper,
    IIdentityService identityService
  )
  {
    _context = context;
    _mapper = mapper;
    _identityService = identityService;
  }

  public async Task<PaginatedList<TodoItemBriefDto>> Handle(
    GetTodoItemsWithPaginationQuery request,
    CancellationToken cancellationToken
  )
  {
    var authorized = await _identityService.AuthorizeAsync(
      ProtectedResourcePolicy.From(
        Resources.TodoResourceItem,
        request.ListId.ToString(),
        $"{Resources.TodoResourceItem}:{Scopes.Read}"
      )
    );

    if (!authorized)
    {
      throw new ForbiddenAccessException();
    }

    return await _context
      .TodoItems.Where(x => x.ListId == request.ListId)
      .OrderBy(x => x.Title)
      .ProjectTo<TodoItemBriefDto>(_mapper.ConfigurationProvider)
      .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
  }
}
