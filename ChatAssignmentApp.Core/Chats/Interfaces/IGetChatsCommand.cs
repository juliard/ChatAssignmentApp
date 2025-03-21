using ChatAssignmentApp.Core.Chats.Models;
using ChatAssignmentApp.Core.Model;

namespace ChatAssignmentApp.Core.Chats.Interfaces
{
    public interface IGetChatsCommand
    {
        CommandResult<List<ChatModel>> Execute(
            Guid shiftId,
            Guid? agentId);
    }
}
