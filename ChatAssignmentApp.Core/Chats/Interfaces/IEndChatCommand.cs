using ChatAssignmentApp.Core.Chats.Models;
using ChatAssignmentApp.Core.Model;

namespace ChatAssignmentApp.Core.Chats.Interfaces
{
    public interface IEndChatCommand
    {
        CommandResult<bool> Execute(
            Guid shiftId,
            Guid chatId);
    }
}
