using CheeseGrater.RpgApi.Application.Common.Interfaces;
using CheeseGrater.RpgApi.Application.Common.Models;
using CheeseGrater.RpgApi.Application.Common.Models.Dto;

namespace CheeseGrater.RpgApi.Infrastructure.Game;

public class WorldStateService : IWorldStateService
{
  private readonly Dictionary<int, CharacterState> _characters = new Dictionary<int, CharacterState>
  {
    {
      1,
      new CharacterState { Id = 1, Name = "admin" }
    },
  };

  public CharacterState? GetCharacter(int id) => _characters.TryGetValue(id, out var c) ? c : null;

  public void UpdateCharacter(CharacterState character) => _characters[character.Id] = character;

  public WorldStateDto GetCurrentState() =>
    new()
    {
      Characters = _characters
        .Values.Select(c => new CharacterDto
        {
          Id = c.Id,
          Name = c.Name,
          X = c.Position.X,
          Y = c.Position.Y,
          Health = c.Health,
        })
        .ToList(),
    };
}
