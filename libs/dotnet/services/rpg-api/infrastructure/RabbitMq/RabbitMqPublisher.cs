using CheeseGrater.RpgApi.Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;

namespace CheeseGrater.RpgApi.Infrastructure.RabbitMq;

public class RabbitMQOptions
{
  public string HostName { get; set; } = "localhost";
  public string UserName { get; set; } = "guest";
  public string Password { get; set; } = "guest";
}

public class RabbitMqPublisher : IAsyncDisposable, IEventPublisher
{
  private readonly ConnectionFactory _factory;
  private readonly ILogger<RabbitMqPublisher> _logger;
  private readonly Lazy<Task> _initializer;
  private readonly object _initLock = new();

  private IConnection _conn;
  private IChannel _channel;

  private const string Exchange = "game.events";

  public RabbitMqPublisher(IOptions<RabbitMQOptions> opts, ILogger<RabbitMqPublisher> logger)
  {
    _logger = logger;
    _factory = new ConnectionFactory
    {
      HostName = opts.Value.HostName,
      UserName = opts.Value.UserName,
      Password = opts.Value.Password,
    };
    _initializer = new Lazy<Task>(() => InitializeAsync(CancellationToken.None));
  }

  public async Task InitializeAsync(CancellationToken cancellationToken)
  {
    lock (_initLock)
    {
      if (_conn != null)
        return;
    }
    try
    {
      _conn = await _factory.CreateConnectionAsync(cancellationToken);
      _channel = await _conn.CreateChannelAsync();
      await _channel.ExchangeDeclareAsync(
        exchange: Exchange,
        type: ExchangeType.Fanout,
        durable: false
      );
      _logger.LogInformation("RabbitMQ connection established");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to start RabbitMQ connection");
      throw;
    }
  }

  public async Task PublishItemEquippedAsync(string playerId, string itemId)
  {
    await _initializer.Value;

    var payload = JsonSerializer.Serialize(
      new
      {
        type = "ItemEquipped",
        playerId,
        itemId,
      }
    );
    var body = System.Text.Encoding.UTF8.GetBytes(payload);
    await _channel!.BasicPublishAsync(
      exchange: Exchange,
      routingKey: "",
      mandatory: false,
      basicProperties: new BasicProperties(),
      body: body,
      CancellationToken.None
    );
    _logger.LogInformation("Published ItemEquipped event: {Player} {Item}", playerId, itemId);
  }

  // Async dispose for proper resource cleanup.
  public async ValueTask DisposeAsync()
  {
    if (_channel != null)
    {
      await Task.Run(async () =>
      {
        await _channel.CloseAsync();
        _channel.Dispose();
      });
    }

    if (_conn != null)
    {
      await _conn.CloseAsync();
      _conn.Dispose();
    }

    GC.SuppressFinalize(this);
  }
}
