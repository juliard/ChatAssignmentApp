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

        public QueueService(
            Configuration configuration,
            IRabbitMQIntegration rabbitMQIntegration)
        {
            _config = configuration;
            _rabbitMQIntegration = rabbitMQIntegration;
        }

        public async Task CreateQueues(
            int maxQueueSize)
        {
            var queuesToCreate = new List<string>()
            {
                _config.RabbitMQConfiguration.MainChatQueueName,
                _config.RabbitMQConfiguration.OverflowChatQueueName,
            };

            await _rabbitMQIntegration.CreateQueues(queuesToCreate, maxQueueSize);
        }

        public async Task DeleteQueues()
        {
            var queuesToDelete = new List<string>()
            {
                _config.RabbitMQConfiguration.MainChatQueueName,
                _config.RabbitMQConfiguration.OverflowChatQueueName,
            };

            await _rabbitMQIntegration.DeleteQueues(queuesToDelete);
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
            return JsonSerializer.Deserialize<Chat>(jsonString);
        }

        public async Task<uint> GetQueueItemCount(
            string queueName)
        {
            return await _rabbitMQIntegration.GetQueueItemCount(queueName);
        }
    }
}
