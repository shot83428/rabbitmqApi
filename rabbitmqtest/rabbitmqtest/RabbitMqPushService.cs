using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class RabbitMqPushService : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" ,
            Port = 5672,
            UserName= "admin",
            Password= "pass.123"
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "testqueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        while (!stoppingToken.IsCancellationRequested)
        {
            var message = $"Hello RabbitMQ! {DateTime.Now}";
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "testqueue", basicProperties: null, body: body);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
