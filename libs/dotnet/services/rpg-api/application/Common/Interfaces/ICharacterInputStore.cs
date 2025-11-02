using CheeseGrater.RpgApi.Application.Common.Models.Dto;

namespace CheeseGrater.RpgApi.Application.Common.Interfaces;

public interface ICharacterInputStore
{
  void SetInput(int characterId, CharacterInputDto input);
  CharacterInputDto? GetInput(int characterId);
  IEnumerable<KeyValuePair<int, CharacterInputDto>> GetAllInputs();
}
