using CheeseGrater.Application.Common.Interfaces;
using CheeseGrater.Core.Application.Common.Interfaces;

namespace CheeseGrater.Application.TodoLists.Commands.CreateTodoList;

public class CreateTodoListCommandValidator : AbstractValidator<CreateTodoListCommand>
{
  private readonly IApplicationDbContext _context;
  private readonly IIdentityService _identityService;

  public CreateTodoListCommandValidator(
    IApplicationDbContext context,
    IIdentityService identityService
  )
  {
    _context = context;
    _identityService = identityService;

    RuleFor(v => v.Title)
      .NotEmpty()
      .MaximumLength(200)
      .MustAsync(BeUniqueTitle)
      .WithMessage("'{PropertyName}' must be unique.")
      .WithErrorCode("Unique");
  }

  public async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
  {
    var userId = _identityService?.UserId ?? throw new InvalidOperationException();
    return !await _context.TodoLists.AnyAsync(
      l => l.Title == title && l.CreatedBy == userId,
      cancellationToken
    );
  }
}
