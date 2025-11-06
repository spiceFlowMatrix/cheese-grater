using CheeseGrater;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

public class GameCommandsService(
  GameWorld world,
  RabbitMqPublisher publisher,
  ILogger<GameCommandsService> logger
) : GameCommandService.GameCommandServiceBase
{
  private readonly GameWorld _world = world;
  private readonly RabbitMqPublisher _publisher = publisher;
  private readonly ILogger<GameCommandsService> _logger = logger;

  public override async Task<Empty> NotifyEquipChange(
    EquipChangeRequest request,
    ServerCallContext context
  )
  {
    _logger.LogInformation(
      "gRPC: NotifyEquipChange player={Player} item={Item}",
      request.PlayerId,
      request.ItemId
    );
    _world.ApplyEquip(request.PlayerId, request.ItemId);
    await _publisher.PublishItemEquippedAsync(request.PlayerId, request.ItemId);
    return new Empty();
  }
}
