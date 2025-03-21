using ChatAssignmentApp.Core.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ChatAssignmentApp.Queuing.Integrations
{
    public class RabbitMQIntegration : IRabbitMQIntegration
    {
        private readonly Configuration _config;

        public RabbitMQIntegration(
            Configuration configuration)
        {
            _config = configuration;
        }

        public async Task CreateQueues(
            List<string> queueNames,
            int maxQueueSize)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config.RabbitMQConfiguration.RabbitMQConnectionName
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var arguments = new Dictionary<string, object?>
            {
                { "x-max-length", maxQueueSize },
                { "x-overflow", "reject-publish" },
            };

            foreach (var queueName in queueNames)
            {
                await channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: arguments);
            }

            await channel.CloseAsync();
            await connection.CloseAsync();
        }

        public async Task DeleteQueues(
            List<string> queueNames)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config.RabbitMQConfiguration.RabbitMQConnectionName
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            foreach (var queueName in queueNames)
            {
                await channel.QueueDeleteAsync(
                    queue: queueName,
                    ifEmpty: true);
            }

            await channel.CloseAsync();
            await connection.CloseAsync();
        }

        public async Task Enqueue(
            string queueName,
            string item)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config.RabbitMQConfiguration.RabbitMQConnectionName
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var body = Encoding.UTF8.GetBytes(item);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                body: body);

            await channel.CloseAsync();
            await connection.CloseAsync();
        }

        public async Task<string> Dequeue(
            string queueName)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config.RabbitMQConfiguration.RabbitMQConnectionName
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var tcs = new TaskCompletionSource<string>();

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                tcs.SetResult(message);
            };

            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer);

            string item = await tcs.Task;

            await channel.CloseAsync();
            await connection.CloseAsync();

            return item;
        }

        public async Task MoveQueueItem(
            string fromQueueName,
            string toQueueName)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config.RabbitMQConfiguration.RabbitMQConnectionName
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var tcs = new TaskCompletionSource<string>();

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                tcs.SetResult(message);
            };

            await channel.BasicConsumeAsync(
                queue: fromQueueName,
                autoAck: false,
                consumer: consumer);

            string item = await tcs.Task;

            var body = Encoding.UTF8.GetBytes(item);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: toQueueName,
                body: body);

            await channel.CloseAsync();
            await connection.CloseAsync();
        }

        public async Task<uint> GetQueueItemCount(
            string queueName)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config.RabbitMQConfiguration.RabbitMQConnectionName
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var queue = await channel.QueueDeclarePassiveAsync(queueName);
            var result = queue.MessageCount;

            await channel.CloseAsync();
            await connection.CloseAsync();

            return result;
        }
    }
}
