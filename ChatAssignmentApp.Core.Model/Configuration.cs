namespace ChatAssignmentApp.Core.Model
{
    public class Configuration
    {
        public RabbitMQConfiguration RabbitMQConfiguration { get; set; }
    }

    public class RabbitMQConfiguration
    {
        public string MainChatQueueName { get; set; } = string.Empty;
        public string OverflowChatQueueName { get; set; } = string.Empty;
        public string RabbitMQConnectionName { get; set; } = string.Empty;
        public string RabbitMQUsername { get; set; } = string.Empty;
        public string RabbitMQPassword { get; set; } = string.Empty;
    }
}
