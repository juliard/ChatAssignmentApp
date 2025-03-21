using ChatAssignmentApp.Domain;
using ChatAssignmentApp.Memory.Services;
using Microsoft.Extensions.Hosting;

namespace ChatAssignmentApp.HostedService.Services
{
    public class ActiveChatMonitorHostedService : BackgroundService
    {
        private readonly IShiftStorageService _shiftStorageService;

        public ActiveChatMonitorHostedService(
            IShiftStorageService shiftStorageService)
        {
            _shiftStorageService = shiftStorageService;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var currentPollTime = DateTime.UtcNow;

            while (!stoppingToken.IsCancellationRequested)
            {
                var shift = _shiftStorageService.GetShift();

                if (shift == null)
                {
                    Console.WriteLine("Active Chat Monitor - There is no shift available. ");
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                var chats = shift.Agents.SelectMany(a => a.Chats).ToList();

                foreach (var chat in chats)
                {
                    var difference = currentPollTime - chat.ChatLastModified;

                    if (difference.TotalSeconds >= 3)
                        chat.SetChatToInactive();
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
