using CheeseGrater.RpgApi.Application.Common.Models.Dto;

namespace CheeseGrater.RpgApi.Application.Common.Interfaces;

public interface ICharacterInputStore
{
  void SetInput(Guid characterId, CharacterInputDto input);
  CharacterInputDto? GetInput(Guid characterId);
  IEnumerable<KeyValuePair<Guid, CharacterInputDto>> GetAllInputs();
}
