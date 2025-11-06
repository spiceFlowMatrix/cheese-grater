using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

public class GameWorld
{
  private readonly ConcurrentDictionary<string, string> _equipped = new();
  private readonly ILogger<GameWorld> _logger;

  public GameWorld(ILogger<GameWorld> logger) => _logger = logger;

  public void ApplyEquip(string playerId, string itemId)
  {
    _equipped[playerId] = itemId;
    _logger.LogInformation("World: Player {Player} equipped {Item}", playerId, itemId);
  }

  public string? GetEquipped(string playerId) =>
    _equipped.TryGetValue(playerId, out var v) ? v : null;
}
