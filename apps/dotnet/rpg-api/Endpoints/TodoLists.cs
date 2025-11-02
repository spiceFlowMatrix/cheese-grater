using Microsoft.AspNetCore.Http.HttpResults;

namespace CheeseGrater.RpgApi.Endpoints;

public class TodoLists : EndpointGroupBase
{
  public override void Map(WebApplication app)
  {
    app.MapGroup(this).RequireAuthorization();
  }
}
