using CheeseGrater.Core.Domain.Common;
using CheeseGrater.RpgApi.Domain.ValueObjects;

namespace CheeseGrater.RpgApi.Domain.Entities;

public class Character : BaseAuditableEntity
{
  public string Name { get; set; } = string.Empty;
  public int Level { get; set; } = 1;
  public int Experience { get; set; } = 0;
  public int Health { get; set; } = 100;
  public float Speed { get; set; } = 5f;

  public required int PlayerId { get; set; }
  public Player Player { get; set; } = null!;

  public Position LastKnownPosition { get; set; } = new(0, 0);
}
