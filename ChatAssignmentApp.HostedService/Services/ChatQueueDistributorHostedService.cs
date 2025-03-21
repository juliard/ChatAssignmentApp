using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Memory.Services;
using ChatAssignmentApp.Queuing.Services;
using Microsoft.Extensions.Hosting;

namespace ChatAssignmentApp.HostedService.Services
{
    public class ChatQueueDistributorHostedService : BackgroundService
    {
        private readonly Configuration _config;

        private readonly IQueueService _queueService;
        private readonly IShiftStorageService _shiftStorageService;

        public ChatQueueDistributorHostedService(
            Configuration config,
            IQueueService queueService,
            IShiftStorageService shiftStorageService)
        {
            _config = config;
            _queueService = queueService;
            _shiftStorageService = shiftStorageService;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            await Task.Delay(60000, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                var shift = _shiftStorageService.GetShift();

                if (shift == null)
                {
                    Console.WriteLine("Chat Queue Distributor - There is no shift available. ");
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                if (shift.IsOverflowAgentsAvailable)
                {
                    var mainQueueItemCount = await _queueService.GetQueueItemCount(
                        _config.RabbitMQConfiguration.MainChatQueueName);

                    if (mainQueueItemCount < shift.MaxChatsToQueue)
                    {
                        var overflowQueueItemCount = await _queueService.GetQueueItemCount(
                            _config.RabbitMQConfiguration.OverflowChatQueueName);

                        if (overflowQueueItemCount > 0)
                            await _queueService.MoveQueueItem();
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
