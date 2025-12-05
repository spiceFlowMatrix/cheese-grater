namespace CheeseGrater.Application.Common.Security;

public sealed class SpaClientOptions
{
  public string ClientId { get; set; } = "todo-web";
  public string RootUrl { get; set; } = "http://localhost:4200";
  public bool RequireHttps { get; set; }
}
