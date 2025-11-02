namespace CheeseGrater.RpgApi.Application.Common.Models.Dto;

public class CharacterInputDto
{
  public bool MoveUp { get; set; }
  public bool MoveDown { get; set; }
  public bool MoveLeft { get; set; }
  public bool MoveRight { get; set; }
  public float Speed { get; set; } = 1.0f;
}
