using CheeseGrater.RpgApi.Application.Common.Interfaces;

namespace CheeseGrater.RpgApi.Application.Game.EquipChange;

public record EquipChangeCommand(string PlayerId, string ItemId) : IRequest;

public class EquipChangeHandler : IRequestHandler<EquipChangeCommand>
{
  private readonly IGameWorld _world;
  private readonly IEventPublisher _publisher;

  public EquipChangeHandler(IGameWorld world, IEventPublisher publisher)
  {
    _world = world;
    _publisher = publisher;
  }

  public async Task Handle(EquipChangeCommand request, CancellationToken cancellationToken)
  {
    _world.ApplyEquip(request.PlayerId, request.ItemId);
    await _publisher.PublishItemEquippedAsync(request.PlayerId, request.ItemId);
  }
}
