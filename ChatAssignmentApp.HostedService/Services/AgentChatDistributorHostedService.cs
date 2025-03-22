﻿using ChatAssignmentApp.Core.Model;
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
            await Task.Delay(60000, stoppingToken);
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

                var chatAssigned = await AssignChat(jrAgents, shift.IsOverflowAgentsAvailable, false);

                if (!chatAssigned)
                    chatAssigned = await AssignChat(midAgents, shift.IsOverflowAgentsAvailable, false);

                if (!chatAssigned)
                    chatAssigned = await AssignChat(srAgents, shift.IsOverflowAgentsAvailable, false);

                if (!chatAssigned)
                    chatAssigned = await AssignChat(leadAgents, shift.IsOverflowAgentsAvailable, false);

                if (!chatAssigned
                    && shift.IsOverflowAgentsAvailable)
                {
                    chatAssigned = await AssignChat(shift.OverflowAgents, shift.IsOverflowAgentsAvailable, true);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task<bool> AssignChat(
            List<Agent> agents,
            bool isOverflowQueueAvailable,
            bool isOverflowAgent)
        {
            if (agents == null || !agents.Any())
                return false;

            int minValue = agents.Min(a => a.Chats.Count);
            var agentToAssign = agents.FirstOrDefault(
                a => a.Chats.Count == minValue);

            if (agentToAssign == null)
                return false;

            if (agentToAssign.Chats.Count == agentToAssign.MaxChatSessions)
                return false;

            var mainQueueItemCount = await _queueService.GetQueueItemCount(
                _config.RabbitMQConfiguration.MainChatQueueName);

            if (mainQueueItemCount > 0)
            {
                var chat = await _queueService.Dequeue(
                    _config.RabbitMQConfiguration.MainChatQueueName);

                if (chat == null)
                    return false;

                var overflowText = isOverflowAgent ? "overflow" : string.Empty;

                Console.WriteLine($"Dequeued chat {chat.ChatId} from main chat queue assigned" +
                    $" to {overflowText} agent {agentToAssign.AgentId} {agentToAssign.AgentSeniorityType}-{agentToAssign.AgentNumber}");

                agentToAssign.AddChat(chat);
            }

            return true;
        }
    }
}
