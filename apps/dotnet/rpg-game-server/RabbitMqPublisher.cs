using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

public class RabbitMqPublisher : IDisposable
{
  private readonly IConnection _conn;
  private readonly RabbitMQ.Client.IModel _channel;
  private readonly ILogger<RabbitMqPublisher> _logger;
  private const string Exchange = "game.events";

  public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger)
  {
    _logger = logger;
    var factory = new ConnectionFactory
    {
      HostName = "localhost",
      UserName = "guest",
      Password = "guest",
    };
    _conn = factory.CreateConnection();
    _channel = _conn.CreateModel();
    _channel.ExchangeDeclare(
      exchange: Exchange,
      type: RabbitMQ.Client.ExchangeType.Fanout,
      durable: false
    );
  }

  public void PublishItemEquipped(string playerId, string itemId)
  {
    var payload = System.Text.Json.JsonSerializer.Serialize(
      new
      {
        type = "ItemEquipped",
        playerId,
        itemId,
      }
    );
    var body = System.Text.Encoding.UTF8.GetBytes(payload);
    _channel.BasicPublish(exchange: Exchange, routingKey: "", basicProperties: null, body);
    _logger.LogInformation("Published ItemEquipped event: {Player} {Item}", playerId, itemId);
  }

  public void Dispose()
  {
    _channel?.Close();
    _conn?.Close();
  }
}