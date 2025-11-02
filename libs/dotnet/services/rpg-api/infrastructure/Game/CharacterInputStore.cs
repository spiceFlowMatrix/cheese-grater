using System.Collections.Concurrent;
using CheeseGrater.RpgApi.Application.Common.Interfaces;
using CheeseGrater.RpgApi.Application.Common.Models.Dto;

public class CharacterInputStore : ICharacterInputStore
{
  private readonly ConcurrentDictionary<int, CharacterInputDto> _inputs = new();

  public void SetInput(int playerId, CharacterInputDto input) => _inputs[playerId] = input;

  public CharacterInputDto? GetInput(int playerId) =>
    _inputs.TryGetValue(playerId, out var input) ? input : null;

  public IEnumerable<KeyValuePair<int, CharacterInputDto>> GetAllInputs() => _inputs;
}
