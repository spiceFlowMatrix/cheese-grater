using CheeseGrater.RpgApi.Application.Common.Models;

namespace CheeseGrater.RpgApi.Application.Common.Interfaces;

public interface IPlayerInputStore
{
  void SetInput(Guid characterId, PlayerInputDto input);
  PlayerInputDto? GetInput(Guid characterId);
  IEnumerable<KeyValuePair<Guid, PlayerInputDto>> GetAllInputs();
}
