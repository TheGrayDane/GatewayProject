using RabbitMQ.Client;
using System.Text;

namespace Gateway.Service.Helpers
{
    public class LogService
    {
        private readonly string _queueName = "log_queue";

        public async Task LogMessageAsync(DateTime logTime, string message)
        {
            var factory = new ConnectionFactory
            {
                HostName = "172.18.0.2",
                Port = 5672,
                UserName = "TheGrayDane",
                Password = "!2HoppeBolde"
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object?> { { "x-queue-type", "quorum" } });

            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: _queueName, body: body);

            Console.WriteLine($" [x] Sent {message}");
        }
    }
}
