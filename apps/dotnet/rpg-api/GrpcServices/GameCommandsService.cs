using CheeseGrater.RpgApi.Application.Game.EquipChange;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
namespace CheeseGrater.RpgApi.Application.Common.Game;

[AllowAnonymous]
public class GameCommandsService : GameCommandService.GameCommandServiceBase
{
  private readonly IMediator _mediator;
  private readonly ILogger<GameCommandsService> _logger;

  public GameCommandsService(IMediator mediator, ILogger<GameCommandsService> logger)
  {
    _mediator = mediator;
    _logger = logger;
  }

  public override async Task<Empty> NotifyEquipChange(
    EquipChangeRequest request,
    ServerCallContext context
  )
  {
    try
    {
      Console.WriteLine($"Received equip change: {request.PlayerId} -> {request.ItemId}");
      await _mediator.Send(new EquipChangeCommand(request.PlayerId, request.ItemId));
      return new Empty();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error in NotifyEquipChange: {ex}");
      throw;
    }

  }
}
