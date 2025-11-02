using CheeseGrater.Core.Application.Common.Models;
using CheeseGrater.RpgApi.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CheeseGrater.RpgApi.Endpoints;

public class TodoItems : EndpointGroupBase
{
  public override void Map(WebApplication app)
  {
    app.MapGroup(this).RequireAuthorization();
  }
}
