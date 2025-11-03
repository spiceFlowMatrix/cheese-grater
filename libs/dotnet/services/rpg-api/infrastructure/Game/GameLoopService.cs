using CheeseGrater.RpgApi.Application.Common.Interfaces;
using CheeseGrater.RpgApi.Application.Common.Models;
using CheeseGrater.RpgApi.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CheeseGrater.RpgApi.Infrastructure.Game
{
  public class GameLoopService : BackgroundService
  {
    private readonly ILogger<GameLoopService> _logger;
    private readonly ICharacterInputStore _inputStore;
    private readonly IWorldStateService _worldState;
    private readonly IMediator _mediator;
    private readonly float _tickRate = 60f;

    public GameLoopService(
      ICharacterInputStore inputStore,
      IWorldStateService worldState,
      IMediator mediator,
      ILogger<GameLoopService> logger
    )
    {
      _inputStore = inputStore;
      _worldState = worldState;
      _logger = logger;
      _mediator = mediator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      var tickDelay = TimeSpan.FromSeconds(1 / _tickRate);

      while (!stoppingToken.IsCancellationRequested)
      {
        await UpdateWorld();
        await Task.Delay(tickDelay, stoppingToken);
      }
    }

    private async Task UpdateWorld()
    {
      List<CharacterState> appliedCharacters = [];
      foreach (var (charId, input) in _inputStore.GetAllInputs())
      {
        var character = _worldState.GetCharacter(charId);
        if (character == null)
          continue;

        float dx = 0,
          dy = 0;
        if (input.MoveUp)
          dy -= 1;
        if (input.MoveDown)
          dy += 1;
        if (input.MoveLeft)
          dx -= 1;
        if (input.MoveRight)
          dx += 1;

        if (dx != 0 || dy != 0)
        {
          var magnitude = MathF.Sqrt(dx * dx + dy * dy);
          dx /= magnitude;
          dy /= magnitude;

          character.Position = new Position(
            character.Position.X + dx * character.Speed / _tickRate,
            character.Position.Y + dy * character.Speed / _tickRate
          );

          _worldState.UpdateCharacter(character);
        }

        appliedCharacters.Add(character);
      }

      await _mediator.Publish(new GameTickNotification { UpdatedCharacters = appliedCharacters });
    }
  }
}
