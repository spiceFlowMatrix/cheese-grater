using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheeseGrater.RpgApi.Application.Common.Interfaces;

/// <summary>
/// Represents the in-memory or persistent game state.
/// </summary>
public interface IGameWorld
{
  /// <summary>
  /// Apply an equip change for a player.
  /// </summary>
  /// <param name="playerId">The player's ID.</param>
  /// <param name="itemId">The item ID being equipped.</param>
  void ApplyEquip(string playerId, string itemId);

  /// <summary>
  /// Get the currently equipped item for a player.
  /// </summary>
  /// <param name="playerId">The player's ID.</param>
  /// <returns>The equipped item ID, or null if none.</returns>
  string? GetEquipped(string playerId);
}
