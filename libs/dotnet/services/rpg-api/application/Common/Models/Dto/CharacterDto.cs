namespace CheeseGrater.RpgApi.Application.Common.Models.Dto;

public class CharacterDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public float X { get; set; }
  public float Y { get; set; }
  public int Experience { get; set; }
  public int Health { get; set; }
}
