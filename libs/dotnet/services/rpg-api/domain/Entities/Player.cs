using CheeseGrater.Core.Domain.Common;

namespace CheeseGrater.RpgApi.Domain.Entities;

public class Player : BaseAuditableEntity
{
  public required string Name { get; set; }
  public required int Level { get; set; }
}
