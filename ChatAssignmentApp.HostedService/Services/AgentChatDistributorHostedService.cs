using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Domain;
using ChatAssignmentApp.Memory.Services;
using ChatAssignmentApp.Queuing.Services;
using Microsoft.Extensions.Hosting;

namespace ChatAssignmentApp.HostedService.Services
{
    public class AgentChatDistributorHostedService : BackgroundService
    {
        private readonly Configuration _config;

        private readonly IQueueService _queueService;
        private readonly IShiftStorageService _shiftStorageService;

        public AgentChatDistributorHostedService(
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
            while (!stoppingToken.IsCancellationRequested)
            {
                var shift = _shiftStorageService.GetShift();

                if (shift == null)
                {
                    Console.WriteLine("Agent Chat Distributor - There is no shift available. ");
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                var jrAgents = shift.Agents.Where(a => a.AgentSeniorityType == AgentSeniorityType.Junior).ToList();
                var midAgents = shift.Agents.Where(a => a.AgentSeniorityType == AgentSeniorityType.Mid).ToList();
                var srAgents = shift.Agents.Where(a => a.AgentSeniorityType == AgentSeniorityType.Senior).ToList();
                var leadAgents = shift.Agents.Where(a => a.AgentSeniorityType == AgentSeniorityType.Lead).ToList();

                var chatAssigned = await AssignChat(jrAgents, shift.IsOverflowAgentsAvailable);

                if (!chatAssigned)
                    chatAssigned = await AssignChat(midAgents, shift.IsOverflowAgentsAvailable);

                if (!chatAssigned)
                    chatAssigned = await AssignChat(srAgents, shift.IsOverflowAgentsAvailable);

                if (!chatAssigned)
                    chatAssigned = await AssignChat(leadAgents, shift.IsOverflowAgentsAvailable);

                if (!chatAssigned
                    && shift.IsOverflowAgentsAvailable)
                {
                    chatAssigned = await AssignChat(shift.OverflowAgents, shift.IsOverflowAgentsAvailable);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task<bool> AssignChat(
            List<Agent> agents,
            bool isOverflowQueueAvailable)
        {
            if (agents == null || !agents.Any())
                return false;

            int minValue = agents.Min(a => a.Chats.Count);
            var agentToAssign = agents.FirstOrDefault(
                a => a.Chats.Count == minValue
                    && a.Chats.Count <= a.MaxChatSessions);

            if (agentToAssign == null)
                return false;

            var mainQueueItemCount = await _queueService.GetQueueItemCount(
                _config.RabbitMQConfiguration.MainChatQueueName);

            if (mainQueueItemCount > 0)
            {
                var chat = await _queueService.Dequeue(_config.RabbitMQConfiguration.MainChatQueueName);
                if (chat != null)
                {
                    Console.WriteLine($"Assigning chat {chat.ChatId} to agent {agentToAssign.AgentSeniorityType}-{agentToAssign.AgentNumber}");
                    agentToAssign.AddChat(chat);
                }

                return true;
            }
            else if (isOverflowQueueAvailable)
            {
                var overflowQueueItemCount = await _queueService.GetQueueItemCount(
                    _config.RabbitMQConfiguration.OverflowChatQueueName);

                if (overflowQueueItemCount > 0)
                {
                    var chat = await _queueService.Dequeue(_config.RabbitMQConfiguration.OverflowChatQueueName);
                    if (chat != null)
                    {
                        Console.WriteLine($"Assigning chat {chat.ChatId} to overflow agent {agentToAssign.AgentSeniorityType}-{agentToAssign.AgentNumber}");
                        agentToAssign.AddChat(chat);
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
