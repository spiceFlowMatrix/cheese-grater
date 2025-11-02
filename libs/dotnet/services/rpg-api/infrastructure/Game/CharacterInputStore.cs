using System.Collections.Concurrent;
using CheeseGrater.RpgApi.Application.Common.Interfaces;
using CheeseGrater.RpgApi.Application.Common.Models.Dto;

public class CharacterInputStore : ICharacterInputStore
{
  private readonly ConcurrentDictionary<Guid, CharacterInputDto> _inputs = new();

  public void SetInput(Guid playerId, CharacterInputDto input) => _inputs[playerId] = input;

  public CharacterInputDto? GetInput(Guid playerId) =>
    _inputs.TryGetValue(playerId, out var input) ? input : null;

  public IEnumerable<KeyValuePair<Guid, CharacterInputDto>> GetAllInputs() => _inputs;
}
