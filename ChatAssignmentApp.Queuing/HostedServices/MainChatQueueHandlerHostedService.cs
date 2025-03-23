using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Domain;
using ChatAssignmentApp.Memory.Services;
using ChatAssignmentApp.Queuing.Services;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ChatAssignmentApp.Queuing.HostedServices
{
    public class MainChatQueueHandlerHostedService : BackgroundService
    {
        private readonly Configuration _config;

        private readonly IQueueService _queueService;
        private readonly IShiftStorageService _shiftStorageService;

        public MainChatQueueHandlerHostedService(
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

                var factory = new ConnectionFactory()
                {
                    HostName = _config.RabbitMQConfiguration.RabbitMQConnectionName
                };

                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        var chat = string.IsNullOrEmpty(message) ? null : JsonSerializer.Deserialize<Chat>(message);

                        if (chat != null)
                        {
                            Console.WriteLine($"{chat.ChatId} picked up. ");

                            ProcessChat(shift, chat);

                            await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                        await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                await channel.BasicConsumeAsync(
                    queue: _config.RabbitMQConfiguration.MainChatQueueName,
                    autoAck: false,
                    consumer: consumer);

                await Task.Delay(1000, stoppingToken);
            }
        }

        private void ProcessChat(
            Shift shift,
            Chat chat)
        {
            var jrAgents = shift.Agents.Where(a => a.AgentSeniorityType == AgentSeniorityType.Junior).ToList();
            var midAgents = shift.Agents.Where(a => a.AgentSeniorityType == AgentSeniorityType.Mid).ToList();
            var srAgents = shift.Agents.Where(a => a.AgentSeniorityType == AgentSeniorityType.Senior).ToList();
            var leadAgents = shift.Agents.Where(a => a.AgentSeniorityType == AgentSeniorityType.Lead).ToList();

            var chatAssigned = AssignChat(jrAgents, chat);

            if (!chatAssigned)
                chatAssigned = AssignChat(midAgents, chat);

            if (!chatAssigned)
                chatAssigned = AssignChat(srAgents, chat);

            if (!chatAssigned)
                chatAssigned = AssignChat(leadAgents, chat);

            if (!chatAssigned
                && shift.IsOverflowAgentsAvailable)
            {
                chatAssigned = AssignChat(shift.OverflowAgents, chat);
            }
        }

        private bool AssignChat(
            List<Agent> agents,
            Chat chat)
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

            agentToAssign.AddChat(chat);

            var overflowAgentText = agentToAssign.IsOverflowAgent ? "overflow" : string.Empty;
            Console.WriteLine($"Dequeued chat {chat.ChatId} and assigned to {overflowAgentText} agent {agentToAssign.AgentId}.");

            return true;
        }
    }
}
