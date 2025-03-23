using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Memory.Services;
using ChatAssignmentApp.Queuing.Services;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ChatAssignmentApp.Queuing.HostedServices
{
    public class OverflowChatQueueHandlerHostedService : BackgroundService
    {
        private readonly Configuration _config;

        private readonly IQueueService _queueService;
        private readonly IShiftStorageService _shiftStorageService;

        public OverflowChatQueueHandlerHostedService(
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

                        await channel.BasicPublishAsync(
                            exchange: string.Empty,
                            routingKey: _config.RabbitMQConfiguration.MainChatQueueName,
                            body: body);

                        Console.WriteLine("moving");

                        await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                        await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                await channel.BasicConsumeAsync(
                    queue: _config.RabbitMQConfiguration.OverflowChatQueueName,
                    autoAck: false,
                    consumer: consumer);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
