using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Domain;
using ChatAssignmentApp.Memory.Services;
using Microsoft.Extensions.Hosting;

namespace ChatAssignmentApp.Core.Chats.HostedServices
{
    public class ActiveChatMonitorHostedService : BackgroundService
    {
        private readonly Configuration _config;
        private readonly IShiftStorageService _shiftStorageService;

        public ActiveChatMonitorHostedService(
            Configuration config,
            IShiftStorageService shiftStorageService)
        {
            _config = config;
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
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                var currentPollTime = DateTime.UtcNow;

                var chatsToUpdate = new List<Chat>();

                if (shift.Agents.Any(a => a.Chats.Any()))
                {
                    chatsToUpdate.AddRange(
                        shift.Agents
                            .Where(a => a.Chats.Any())
                            .SelectMany(a => a.Chats)
                            .OrderBy(a => a.ChatLastModified)
                            .ToList());
                }

                if (shift.IsOverflowAgentsAvailable
                    && shift.OverflowAgents.Any(a => a.Chats.Any()))
                {
                    chatsToUpdate.AddRange(
                        shift.OverflowAgents
                            .Where(a => a.Chats.Any())
                            .SelectMany(a => a.Chats)
                            .OrderBy(a => a.ChatLastModified)
                            .ToList());
                }

                foreach (var chat in chatsToUpdate)
                {
                    var difference = currentPollTime - chat.ChatLastModified;

                    if (difference.TotalSeconds >= _config.RabbitMQConfiguration.ChatInactivityExpiryInSeconds
                        && chat.IsChatActive)
                    {
                        chat.SetChatToInactive();
                        Console.WriteLine($"Setting {chat.ChatId} to inactive");
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
