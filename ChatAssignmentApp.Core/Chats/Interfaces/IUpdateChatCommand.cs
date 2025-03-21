using ChatAssignmentApp.Core.Chats.Models;
using ChatAssignmentApp.Core.Model;

namespace ChatAssignmentApp.Core.Chats.Interfaces
{
    public interface IUpdateChatCommand
    {
        CommandResult<ChatModel> Execute(
            Guid shiftId,
            Guid chatId);
    }
}
