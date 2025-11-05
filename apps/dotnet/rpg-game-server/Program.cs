// apps/rpg-game-server/Program.cs
using System;
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
        end.MapGrpcService<GameCommandService>();
      });
    });
  });

var host = builder.Build();
await host.RunAsync();

// ---- Small supporting types (could be extracted to separate files) ----

public class GameServerOptions
{
  public string Host { get; set; } = "0.0.0.0";
  public ushort Port { get; set; } = 7777;
  public int MaxPeers { get; set; } = 64;
}

public class GameCommandsServiceBase(
  GameWorld world,
  RabbitMqPublisher publisher,
  ILogger<GameCommandsServiceBase> logger
) : CheeseGrater.GameCommandService
{
  private readonly GameWorld _world = world;
  private readonly RabbitMqPublisher _publisher = publisher;
  private readonly ILogger<GameCommandsServiceBase> _logger = logger;

  public override Task<Empty> NotifyEquipChange(
    EquipChangeRequest request,
    ServerCallContext context
  )
  {
    _logger.LogInformation(
      "gRPC: NotifyEquipChange player={Player} item={Item}",
      request.PlayerId,
      request.ItemId
    );
    _world.ApplyEquip(request.PlayerId, request.ItemId);
    _publisher.PublishItemEquipped(request.PlayerId, request.ItemId);
    return Task.FromResult(new Empty());
  }
}

public class GameWorld
{
  private readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> _equipped =
    new();
  private readonly ILogger<GameWorld> _logger;

  public GameWorld(ILogger<GameWorld> logger) => _logger = logger;

  public void ApplyEquip(string playerId, string itemId)
  {
    _equipped[playerId] = itemId;
    _logger.LogInformation("World: Player {Player} equipped {Item}", playerId, itemId);
  }

  public string? GetEquipped(string playerId) =>
    _equipped.TryGetValue(playerId, out var v) ? v : null;
}

public class EnetHostedService : Microsoft.Extensions.Hosting.BackgroundService
{
  private readonly Microsoft.Extensions.Options.IOptions<GameServerOptions> _opts;
  private readonly ILogger<EnetHostedService> _logger;

  public EnetHostedService(
    Microsoft.Extensions.Options.IOptions<GameServerOptions> opts,
    ILogger<EnetHostedService> logger
  )
  {
    _opts = opts;
    _logger = logger;
  }

  protected override Task ExecuteAsync(CancellationToken stoppingToken)
  {
    // Start ENet loop on separate thread/task
    return Task.Run(
      () =>
      {
        ENet.Library.Initialize();
        using var host = new ENet.Host();
        var address = new ENet.Address { Port = _opts.Value.Port };
        host.Create(address, _opts.Value.MaxPeers);
        _logger.LogInformation("ENet server started on UDP port {Port}", _opts.Value.Port);

        var evt = new ENet.Event();
        while (!stoppingToken.IsCancellationRequested)
        {
          while (host.Service(15, out evt) > 0)
          {
            switch (evt.Type)
            {
              case ENet.EventType.Connect:
                _logger.LogInformation("ENet: Peer connected: {PeerId}", evt.Peer.ID);
                break;
              case ENet.EventType.Receive:
                var msg = System.Text.Encoding.UTF8.GetString(evt.Packet.GetBytes());
                _logger.LogInformation("ENet: Received from {Peer}: {Msg}", evt.Peer.ID, msg);
                // echo for demo
                var resp = System.Text.Encoding.UTF8.GetBytes("Echo: " + msg);
                evt.Peer.Send(
                  0,
                  ref new ENet.Packet { UserData = resp, Flags = ENet.PacketFlags.Reliable }
                );
                evt.Packet.Dispose();
                break;
              case ENet.EventType.Disconnect:
                _logger.LogInformation("ENet: Peer disconnected: {Peer}", evt.Peer.ID);
                break;
            }
          }
          Thread.Sleep(1);
        }

        host.Flush();
        ENet.Library.Deinitialize();
      },
      stoppingToken
    );
  }
}
// Console.WriteLine("See apps/dotnet/rpg-game-server/RabbitMqPublisher.cs for the RabbitMqPublisher implementation.");
