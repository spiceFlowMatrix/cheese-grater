using CheeseGrater.RpgApi.Application.Common.Models;
using CheeseGrater.RpgApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace CheeseGrater.RpgApi.Notifiers;

[SignalRHub]
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

  [return: SignalRReturn(typeof(GameTickNotification))]
  public async Task Handle(
    [SignalRHidden] GameTickNotification notification,
    [SignalRHidden] CancellationToken cancellationToken
  )
  {
    // Broadcast to all connected clients
    await _hubContext.Clients.All.SendAsync(
      "GameTick",
      notification.UpdatedCharacters,
      cancellationToken
    );
  }
}
