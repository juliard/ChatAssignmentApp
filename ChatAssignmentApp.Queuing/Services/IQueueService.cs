using ChatAssignmentApp.Domain;

namespace ChatAssignmentApp.Queuing.Services
{
    public interface IQueueService
    {
        Task CreateQueues(
            int maxQueueSize);

        Task DeleteQueues();

        Task Enqueue(
            string queueName,
            Chat chatToQueue);

        Task<Chat?> Dequeue(
            string queueName);
        Task<uint> GetQueueItemCount(
            string queueName);
    }
}
