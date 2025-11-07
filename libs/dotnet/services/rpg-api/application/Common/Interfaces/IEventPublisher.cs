using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheeseGrater.RpgApi.Application.Common.Interfaces;
/// <summary>
/// Publishes domain or game events to external systems (RabbitMQ, Kafka, etc.).
/// </summary>
public interface IEventPublisher
{
  /// <summary>
  /// Publish a player equipped an item event.
  /// </summary>
  /// <param name="playerId">The player who equipped the item.</param>
  /// <param name="itemId">The item equipped.</param>
  Task PublishItemEquippedAsync(string playerId, string itemId);
}

