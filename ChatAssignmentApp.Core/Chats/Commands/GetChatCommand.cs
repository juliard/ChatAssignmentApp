using ChatAssignmentApp.Core.Chats.Interfaces;
using ChatAssignmentApp.Core.Chats.Models;
using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Memory.Services;

namespace ChatAssignmentApp.Core.Chats.Commands
{
    public class GetChatCommand : IGetChatCommand
    {
        private readonly IShiftStorageService _shiftStorageService;

        public GetChatCommand(
            IShiftStorageService shiftStorageService)
        {
            _shiftStorageService = shiftStorageService;
        }

        public CommandResult<ChatModel> Execute(
            Guid shiftId,
            Guid chatId)
        {
            var shift = _shiftStorageService.GetShift(shiftId);

            if (shift == null)
                return new CommandResult<ChatModel>(false, "The shift is not found. ");

            var chat = shift.Agents
                .Where(a => a.Chats.Any(b => b.ChatId == chatId))
                .FirstOrDefault()?
                .Chats
                .FirstOrDefault(a => a.ChatId == chatId);

            if (chat == null
                && shift.IsOverflowAgentsAvailable)
            {
                chat = shift.OverflowAgents
                    .Where(a => a.Chats.Any(b => b.ChatId == chatId))
                    .FirstOrDefault()?
                    .Chats
                    .FirstOrDefault(a => a.ChatId == chatId);
            }

            if (chat == null)
                return new CommandResult<ChatModel>(false, "The chat is not found. ");

            return new CommandResult<ChatModel>(
                new ChatModel(chat));
        }
    }
}
