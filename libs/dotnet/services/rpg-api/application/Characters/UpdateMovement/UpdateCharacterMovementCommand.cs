using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseGrater.RpgApi.Application.Common.Interfaces;
using CheeseGrater.RpgApi.Application.Common.Models.Dto;

namespace CheeseGrater.RpgApi.Application.Characters.UpdateMovement;

public record UpdateCharacterMovementCommand : IRequest
{
  public int PlayerId { get; init; }

  // Direction flags or vector
  public bool MoveUp { get; init; }
  public bool MoveDown { get; init; }
  public bool MoveLeft { get; init; }
  public bool MoveRight { get; init; }

  // Optional: movement speed multiplier
  public float Speed { get; init; } = 1.0f;
}

public class UpdateCharacterMovementCommandHandler : IRequestHandler<UpdateCharacterMovementCommand>
{
  private readonly ICharacterInputStore _inputStore;

  public UpdateCharacterMovementCommandHandler(ICharacterInputStore inputStore)
  {
    _inputStore = inputStore;
  }

  public async Task Handle(
    UpdateCharacterMovementCommand request,
    CancellationToken cancellationToken
  )
  {
    // Update the input state for this player
    _inputStore.SetInput(
      request.PlayerId,
      new CharacterInputDto
      {
        MoveUp = request.MoveUp,
        MoveDown = request.MoveDown,
        MoveLeft = request.MoveLeft,
        MoveRight = request.MoveRight,
        Speed = request.Speed,
      }
    );
  }
}
