using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Domain;
using ChatAssignmentApp.Queuing.Integrations;
using System.Text.Json;

namespace ChatAssignmentApp.Queuing.Services
{
    public class QueueService : IQueueService
    {
        private readonly Configuration _config;
        private readonly IRabbitMQIntegration _rabbitMQIntegration;

        private readonly List<string> _queues = [];

        public QueueService(
            Configuration configuration,
            IRabbitMQIntegration rabbitMQIntegration)
        {
            _config = configuration;
            _rabbitMQIntegration = rabbitMQIntegration;

            _queues.Add(_config.RabbitMQConfiguration.MainChatQueueName);
        }

        public async Task CreateQueues(
            int maxQueueSize,
            bool isOverflowChatAvailable)
        {
            try
            {
                if (isOverflowChatAvailable)
                    _queues.Add(_config.RabbitMQConfiguration.OverflowChatQueueName);

                await _rabbitMQIntegration.CreateQueues(_queues, maxQueueSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Creation of chat queues failed - {ex.ToString()}");
            }
        }

        public async Task DeleteQueues(
            bool isOverflowChatAvailable)
        {
            try
            {
                if (isOverflowChatAvailable)
                    _queues.Add(_config.RabbitMQConfiguration.OverflowChatQueueName);

                await _rabbitMQIntegration.DeleteQueues(_queues);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deletion of chat queues failed - {ex.ToString()}");
            }
        }

        public async Task Enqueue(
            string queueName,
            Chat chatToQueue)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(chatToQueue);
                await _rabbitMQIntegration.Enqueue(queueName, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Enqueue on queue {queueName} failed - {ex.ToString()}");
            }
        }

        public async Task<Chat?> Dequeue(
            string queueName)
        {
            try
            {
                var jsonString = await _rabbitMQIntegration.Dequeue(queueName);
                return string.IsNullOrWhiteSpace(jsonString) ? null : JsonSerializer.Deserialize<Chat>(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dequeue on queue {queueName} failed - {ex.ToString()}");
                return null;
            }
        }

        public async Task<Chat?> MoveQueueItem(
            string fromQueueName,
            string toQueueName)
        {
            try
            {
                var chat = await Dequeue(fromQueueName);

                if (chat == null)
                    return null;

                await Enqueue(toQueueName, chat);
                return chat;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public async Task<uint> GetQueueItemCount(
            string queueName)
        {
            try
            {
                return await _rabbitMQIntegration.GetQueueItemCount(queueName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }
    }
}
