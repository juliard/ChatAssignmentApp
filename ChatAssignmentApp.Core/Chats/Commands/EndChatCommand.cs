using ChatAssignmentApp.Core.Chats.Interfaces;
using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Memory.Services;

namespace ChatAssignmentApp.Core.Chats.Commands
{
    public class EndChatCommand : IEndChatCommand
    {
        private readonly IShiftStorageService _shiftStorageService;

        public EndChatCommand(
            IShiftStorageService shiftStorageService)
        {
            _shiftStorageService = shiftStorageService;
        }

        public CommandResult<bool> Execute(
            Guid shiftId,
            Guid chatId)
        {
            var shift = _shiftStorageService.GetShift(shiftId);

            if (shift == null)
                return new CommandResult<bool>(false, "The shift is not found. ");

            var agent = shift.Agents
                .Where(a => a.Chats.Any(b => b.ChatId == chatId))
                .FirstOrDefault();

            if (agent == null
                && shift.IsOverflowAgentsAvailable)
            {
                agent = shift.OverflowAgents
                    .Where(a => a.Chats.Any(b => b.ChatId == chatId))
                    .FirstOrDefault();

                if (agent == null)
                    return new CommandResult<bool>(false, "The agent owning the chat is not found. ");
            }

            var chat = agent?.Chats.FirstOrDefault(a => a.ChatId == chatId);

            if (chat == null)
                return new CommandResult<bool>(false, "The chat is not found. ");

            agent?.Chats.Remove(chat);

            return new CommandResult<bool>(true);
        }
    }
}
