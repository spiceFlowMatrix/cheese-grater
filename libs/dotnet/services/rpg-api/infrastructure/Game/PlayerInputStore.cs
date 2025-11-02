using System.Collections.Concurrent;
using CheeseGrater.RpgApi.Application.Common.Interfaces;
using CheeseGrater.RpgApi.Application.Common.Models;

public class PlayerInputStore : IPlayerInputStore
{
  private readonly ConcurrentDictionary<Guid, PlayerInputDto> _inputs = new();

  public void SetInput(Guid playerId, PlayerInputDto input) => _inputs[playerId] = input;

  public PlayerInputDto? GetInput(Guid playerId) =>
    _inputs.TryGetValue(playerId, out var input) ? input : null;

  public IEnumerable<KeyValuePair<Guid, PlayerInputDto>> GetAllInputs() => _inputs;
}
