using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Models;
using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;
using CheeseGrater.Core.Domain.Enums;

namespace CheeseGrater.Application.TodoLists.Queries.GetTodos;

[AuthorizeProtectedResource(Resources.TodoResource, $"{Resources.TodoResource}:{Scopes.List}")]
public record GetTodosQuery : IRequest<TodosVm>;

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, TodosVm>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;
  private readonly IIdentityService _identityService;

  public GetTodosQueryHandler(
    IApplicationDbContext context,
    IMapper mapper,
    IIdentityService identityService
  )
  {
    _context = context;
    _mapper = mapper;
    _identityService = identityService;
  }

  public async Task<TodosVm> Handle(GetTodosQuery request, CancellationToken cancellationToken)
  {
    var userId = _identityService?.UserId ?? throw new InvalidOperationException();
    return new TodosVm
    {
      PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
        .Cast<PriorityLevel>()
        .Select(p => new LookupDto { Id = (int)p, Title = p.ToString() })
        .ToList(),

      Lists = await _context
        .TodoLists.AsNoTracking()
        .Where((e) => e.CreatedBy == userId)
        .ProjectTo<TodoListDto>(_mapper.ConfigurationProvider)
        .OrderBy(t => t.Title)
        .ToListAsync(cancellationToken),
    };
  }
}
