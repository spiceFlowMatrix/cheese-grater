using CheeseGrater;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder(args)
  .ConfigureAppConfiguration(
    (ctx, cfg) =>
    {
      cfg.AddJsonFile("appsettings.json", optional: true);
      cfg.AddEnvironmentVariables();
    }
  )
  .ConfigureLogging(logging =>
  {
    logging.ClearProviders();
    logging.AddConsole();
  })
  .ConfigureServices(
    (context, services) =>
    {
      // bind config
      services.Configure<GameServerOptions>(context.Configuration.GetSection("GameServer"));
      services.Configure<RabbitMQOptions>(context.Configuration.GetSection("RabbitMQ"));

      // core game services
      services.AddSingleton<GameWorld>();
      services.AddSingleton<RabbitMqPublisher>();
      services.AddHostedService<EnetHostedService>();

      // gRPC endpoints will be added in WebHost config below
    }
  )
  .ConfigureWebHostDefaults(web =>
  {
    web.ConfigureKestrel(opts => opts.ListenLocalhost(50051)); // gRPC control plane
    web.ConfigureServices(s => s.AddGrpc());
    web.Configure(app =>
    {
      app.UseRouting();
      app.UseEndpoints(end =>
      {
        end.MapGrpcService<GameCommandsService>();
      });
    });
  });

var host = builder.Build();
await host.RunAsync();
