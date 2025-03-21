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
            while (!stoppingToken.IsCancellationRequested)
            {
                var shift = _shiftStorageService.GetShift();

                if (shift == null)
                {
                    Console.WriteLine("Active Chat Monitor - There is no shift available. ");
                    await Task.Delay(30000, stoppingToken);
                    continue;
                }

                var currentPollTime = DateTime.UtcNow;

                var chats = shift.Agents
                    .SelectMany(a => a.Chats)
                    .OrderBy(a => a.ChatLastModified)
                    .ToList();

                foreach (var chat in chats)
                {
                    var difference = currentPollTime - chat.ChatLastModified;

                    if (difference.TotalSeconds >= 60)
                    {
                        chat.SetChatToInactive();
                        Console.WriteLine($"Setting {chat.ChatId} to inactive");
                    }
                }

                await Task.Delay(30000, stoppingToken);
            }
        }
    }
}
