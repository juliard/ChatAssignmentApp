namespace ChatAssignmentApp.Queuing.Integrations
{
    public interface IRabbitMQIntegration
    {
        Task CreateQueues(
            List<string> queueNames,
            int maxQueueSize);

        Task DeleteQueues(
            List<string> queueNames);

        Task Enqueue(
            string queueName,
            string item);

        Task<string> Dequeue(
            string queueName);

        Task MoveQueueItem(
            string fromQueueName,
            string toQueueName);

        Task<uint> GetQueueItemCount(
            string queueName);
    }
}
