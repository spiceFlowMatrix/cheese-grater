using CheeseGrater.Infrastructure.Data;
using CheeseGrater.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (
  app.Environment.IsDevelopment()
  && Environment.GetEnvironmentVariable("NSWAG_RUNNING") != "true"
)
{
  await app.InitialiseDatabaseAsync();

  var isNswagProcess =
    AppContext.BaseDirectory.Contains("nswag", StringComparison.OrdinalIgnoreCase)
    || AppDomain.CurrentDomain.FriendlyName.Contains("NSwag", StringComparison.OrdinalIgnoreCase);

  if (builder.Configuration.GetValue("Keycloak:SeedOnStartup", true) && !isNswagProcess)
  {
    await app.InitialiseKeycloakAsync();
  }
}
else
{
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
  settings.Path = "/api";
  settings.DocumentPath = "/api/specification.json";
});

app.MapFallbackToFile("index.html");

app.UseExceptionHandler(options => { });

app.UseCors("SpaCors");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!").RequireAuthorization();

app.MapEndpoints();

app.Run();
