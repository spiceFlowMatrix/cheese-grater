using CheeseGrater.Core.Domain.Common;

namespace CheeseGrater.RpgApi.Domain.Entities;

public class Player : BaseAuditableEntity
{
  public required string Username { get; set; } = string.Empty;

  public IList<Character> Characters { get; private set; } = new List<Character>();
}
