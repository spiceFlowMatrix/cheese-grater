using CheeseGrater.Core.Domain.Common;
using CheeseGrater.RpgApi.Domain.Entities;

namespace CheeseGrater.RpgApi.Application.Common.Models;

public class GameTickNotification : BaseEvent
{
  public List<CharacterState> UpdatedCharacters { get; set; } = [];
}
