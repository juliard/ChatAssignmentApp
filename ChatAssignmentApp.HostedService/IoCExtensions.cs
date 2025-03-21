using ChatAssignmentApp.HostedService.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChatAssignmentApp.HostedService
{
    public static class IoCExtensions
    {
        public static void InjectHostedServices(this IServiceCollection services)
        {
            services.AddHostedService<ActiveChatMonitorHostedService>();
            services.AddHostedService<AgentChatDistributorHostedService>();
            // services.AddHostedService<ChatQueueDistributorHostedService>();
            services.AddHostedService<RemoveInactiveChatMonitorHostedService>();
        }
    }
}
