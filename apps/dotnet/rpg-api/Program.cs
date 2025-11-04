using CheeseGrater.RpgApi.Hubs;
using CheeseGrater.RpgApi.Infrastructure.Data;
using CheeseGrater.RpgApi.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApplicationServices();
builder.AddRpgInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (
  app.Environment.IsDevelopment()
  && Environment.GetEnvironmentVariable("NSWAG_RUNNING") != "true"
)
{
  await app.InitialiseDatabaseAsync();
  //   await app.InitialiseKeycloakAsync();
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

app.UseSwagger();
app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/swagger/signalr-v1/swagger.json", "CheeseGrater SignalR API V1");
  c.RoutePrefix = "swagger-signalr";
});

app.MapFallbackToFile("index.html");

app.UseExceptionHandler(options => { });

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!").RequireAuthorization();

app.MapEndpoints().MapHub<GameHub>("/game");

app.Run();
