namespace CheeseGrater.Core.Application.Common.Interfaces;

public interface IRequestWithResourceId : IRequest
{
    string ResourceId { get; }
}
