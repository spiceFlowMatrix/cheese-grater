using CheeseGrater.RpgApi.Application.Characters.UpdateMovement;
using CheeseGrater.RpgApi.Application.Common.Models;
using CheeseGrater.RpgApi.Application.Common.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CheeseGrater.RpgApi.Hubs;

[AllowAnonymous]
public class GameHub : Hub
{
  private readonly IMediator _mediator;
  private readonly ILogger<GameHub> _logger;

  public GameHub(IMediator mediator, ILogger<GameHub> logger)
  {
    _mediator = mediator;
    _logger = logger;
  }

  public async Task SendPlayerInput(int playerId, int characterId, CharacterInputDto input)
  {
    _logger.LogInformation("SendPlayerInput");
    // Wrap inputs in a command
    var command = new UpdateCharacterMovementCommand
    {
      PlayerId = playerId,
      CharacterId = characterId,
      MoveUp = input.MoveUp,
      MoveDown = input.MoveDown,
      MoveLeft = input.MoveLeft,
      MoveRight = input.MoveRight,
    };

    await _mediator.Send(command);
  }
}
