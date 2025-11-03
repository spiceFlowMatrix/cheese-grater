using CheeseGrater.RpgApi.Application.Common.Models;
using CheeseGrater.RpgApi.Application.Common.Models.Dto;

namespace CheeseGrater.RpgApi.Application.Common.Interfaces;

public interface IWorldStateService
{
  CharacterState? GetCharacter(int id);
  void UpdateCharacter(CharacterState character);
  WorldStateDto GetCurrentState();
}
