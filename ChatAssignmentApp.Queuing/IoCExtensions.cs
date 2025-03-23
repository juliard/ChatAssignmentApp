using ChatAssignmentApp.Queuing.HostedServices;
using ChatAssignmentApp.Queuing.Integrations;
using ChatAssignmentApp.Queuing.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChatAssignmentApp.Queuing
{
    public static class IoCExtensions
    {
        public static void InjectQueuing(this IServiceCollection services)
        {
            services.AddTransient<IRabbitMQIntegration, RabbitMQIntegration>();
            services.AddTransient<IQueueService, QueueService>();

            services.AddHostedService<MainChatQueueHandlerHostedService>();
            services.AddHostedService<OverflowChatQueueHandlerHostedService>();
        }
    }
}
