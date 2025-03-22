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

        Task<Chat?> Dequeue(
            string queueName);

        Task<Chat?> MoveQueueItem(
            string fromQueueName,
            string toQueueName);

        Task<uint> GetQueueItemCount(
            string queueName);
    }
}
