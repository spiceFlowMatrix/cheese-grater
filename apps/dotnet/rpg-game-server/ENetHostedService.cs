using System.Text;
using ENet;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class GameServerOptions
{
  public string Host { get; set; } = "0.0.0.0";
  public ushort Port { get; set; } = 7777;
  public int MaxPeers { get; set; } = 64;
}

public class EnetHostedService : BackgroundService
{
  private readonly IOptions<GameServerOptions> _opts;
  private readonly ILogger<EnetHostedService> _logger;

  public EnetHostedService(IOptions<GameServerOptions> opts, ILogger<EnetHostedService> logger)
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
        Library.Initialize();
        using var host = new ENet.Host();
        var address = new Address { Port = _opts.Value.Port };
        host.Create(address, _opts.Value.MaxPeers);
        _logger.LogInformation("ENet server started on UDP port {Port}", _opts.Value.Port);

        var evt = new Event();
        while (!stoppingToken.IsCancellationRequested)
        {
          while (host.Service(15, out evt) > 0)
          {
            switch (evt.Type)
            {
              case EventType.Connect:
                _logger.LogInformation("ENet: Peer connected: {PeerId}", evt.Peer.ID);
                break;
              case EventType.Receive:
                var buffer = new byte[evt.Packet.Length];
                evt.Packet.CopyTo(buffer);
                var msg = Encoding.UTF8.GetString(buffer);
                _logger.LogInformation("ENet: Received from {Peer}: {Msg}", evt.Peer.ID, msg);
                // echo for demo
                var respBytes = Encoding.UTF8.GetBytes("Echo: " + msg);
                var respPacket = default(Packet);
                respPacket.Create(respBytes, PacketFlags.Reliable);
                evt.Peer.Send(0, ref respPacket);
                evt.Packet.Dispose();
                break;
              case EventType.Disconnect:
                _logger.LogInformation("ENet: Peer disconnected: {Peer}", evt.Peer.ID);
                break;
            }
          }
          Thread.Sleep(1);
        }

        host.Flush();
        Library.Deinitialize();
      },
      stoppingToken
    );
  }
}
