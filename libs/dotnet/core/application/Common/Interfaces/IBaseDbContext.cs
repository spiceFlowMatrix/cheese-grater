namespace CheeseGrater.Core.Application.Common.Interfaces;

public interface IBaseDbContext
{
  Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
