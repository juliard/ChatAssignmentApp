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
            if (isOverflowChatAvailable)
                _queues.Add(_config.RabbitMQConfiguration.OverflowChatQueueName);

            await _rabbitMQIntegration.CreateQueues(_queues, maxQueueSize);
        }

        public async Task DeleteQueues(
            bool isOverflowChatAvailable)
        {
            if (isOverflowChatAvailable)
                _queues.Add(_config.RabbitMQConfiguration.OverflowChatQueueName);

            await _rabbitMQIntegration.DeleteQueues(_queues);
        }

        public async Task Enqueue(
            string queueName,
            Chat chatToQueue)
        {
            string jsonString = JsonSerializer.Serialize(chatToQueue);
            await _rabbitMQIntegration.Enqueue(queueName, jsonString);
        }

        public async Task<Chat?> Dequeue(
            string queueName)
        {
            var jsonString = await _rabbitMQIntegration.Dequeue(queueName);
            return string.IsNullOrWhiteSpace(jsonString) ? null : JsonSerializer.Deserialize<Chat>(jsonString);
        }

        public async Task MoveQueueItem()
        {
            await _rabbitMQIntegration.MoveQueueItem(
                _config.RabbitMQConfiguration.MainChatQueueName,
                _config.RabbitMQConfiguration.OverflowChatQueueName);
        }

        public async Task<uint> GetQueueItemCount(
            string queueName)
        {
            return await _rabbitMQIntegration.GetQueueItemCount(queueName);
        }
    }
}
