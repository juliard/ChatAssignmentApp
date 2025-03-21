using ChatAssignmentApp.Core.Chats.Commands;
using ChatAssignmentApp.Core.Chats.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ChatAssignmentApp.Core.Chats
{
    public static class IoCExtensions
    {
        public static void AddChatCommands(this IServiceCollection services)
        {
            services.AddTransient<ICreateChatCommand, CreateChatCommand>();
            services.AddTransient<IEndChatCommand, EndChatCommand>();
            services.AddTransient<IGetChatCommand, GetChatCommand>();
            services.AddTransient<IGetChatsCommand, GetChatsCommand>();
            services.AddTransient<IUpdateChatCommand, UpdateChatCommand>();
        }
    }
}
