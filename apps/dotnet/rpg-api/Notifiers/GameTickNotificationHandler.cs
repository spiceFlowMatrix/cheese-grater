using CheeseGrater.RpgApi.Application.Common.Models;
using CheeseGrater.RpgApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CheeseGrater.RpgApi.Notifiers;

public class GameTickNotificationHandler : INotificationHandler<GameTickNotification>
{
  private readonly IHubContext<GameHub> _hubContext;
  private readonly ILogger<GameTickNotificationHandler> _logger;

  public GameTickNotificationHandler(
    IHubContext<GameHub> hubContext,
    ILogger<GameTickNotificationHandler> logger
  )
  {
    _hubContext = hubContext;
    _logger = logger;
  }

  public async Task Handle(GameTickNotification notification, CancellationToken cancellationToken)
  {
    // Broadcast to all connected clients
    await _hubContext.Clients.All.SendAsync(
      "GameTick",
      notification.UpdatedCharacters,
      cancellationToken
    );
    _logger.LogInformation("GameTick broadcasted to clients via IHubContext.");
  }
}
