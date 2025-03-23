using ChatAssignmentApp.Memory.Services;
using Microsoft.Extensions.Hosting;

namespace ChatAssignmentApp.Core.Chats.HostedServices
{
    public class RemoveInactiveChatMonitorHostedService : BackgroundService
    {
        private readonly IShiftStorageService _shiftStorageService;

        public RemoveInactiveChatMonitorHostedService(
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
                    Console.WriteLine("Remove Inactive Chat Monitor - There is no shift available. ");
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                foreach (var agent in shift.Agents)
                {
                    if (agent.Chats.Any(a => !a.IsChatActive))
                    {
                        agent.Chats.RemoveAll(a => !a.IsChatActive);
                        Console.WriteLine($"Inactive chats removed for agent {agent.AgentId} {agent.AgentSeniorityType}-{agent.AgentNumber}");
                    }
                }

                foreach (var agent in shift.OverflowAgents)
                {
                    if (agent.Chats.Any(a => !a.IsChatActive))
                    {
                        agent.Chats.RemoveAll(a => !a.IsChatActive);
                        Console.WriteLine($"Inactive chats removed for overflow agent {agent.AgentId} {agent.AgentSeniorityType}-{agent.AgentNumber}");
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
