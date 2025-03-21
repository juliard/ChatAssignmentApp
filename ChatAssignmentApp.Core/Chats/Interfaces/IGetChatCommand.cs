using ChatAssignmentApp.Core.Chats.Models;
using ChatAssignmentApp.Core.Model;

namespace ChatAssignmentApp.Core.Chats.Interfaces
{
    public interface IGetChatCommand
    {
        CommandResult<ChatModel> Execute(
            Guid shiftId,
            Guid chatId);
    }
}
