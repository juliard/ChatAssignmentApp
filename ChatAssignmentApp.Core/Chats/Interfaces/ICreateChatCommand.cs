using ChatAssignmentApp.Core.Chats.Models;
using ChatAssignmentApp.Core.Model;

namespace ChatAssignmentApp.Core.Chats.Interfaces
{
    public interface ICreateChatCommand
    {
        Task<CommandResult<bool>> ExecuteAsync(
            Guid shiftId,
            CreateChatModel model);
    }
}
