using ChatAssignmentApp.Domain;

namespace ChatAssignmentApp.Queuing.Services
{
    public interface IQueueService
    {
        Task CreateQueues(
            int maxQueueSize,
            bool isOverflowChatAvailable);

        Task DeleteQueues(
            bool isOverflowChatAvailable);

        Task Enqueue(
            string queueName,
            Chat chatToQueue);

        Task<uint> GetQueueItemCount(
            string queueName);
    }
}
