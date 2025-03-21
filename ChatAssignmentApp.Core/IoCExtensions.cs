using ChatAssignmentApp.Core.Chats;
using ChatAssignmentApp.Core.Shifts;
using Microsoft.Extensions.DependencyInjection;

namespace ChatAssignmentApp.Core
{
    public static class IoCExtensions
    {
        public static void InjectCore(this IServiceCollection services)
        {
            services.AddChatCommands();
            services.AddShiftCommands();
        }
    }
}
