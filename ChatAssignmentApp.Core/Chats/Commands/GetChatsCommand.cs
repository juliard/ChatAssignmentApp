using ChatAssignmentApp.Core.Chats.Interfaces;
using ChatAssignmentApp.Core.Chats.Models;
using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Memory.Services;

namespace ChatAssignmentApp.Core.Chats.Commands
{
    public class GetChatsCommand : IGetChatsCommand
    {
        private readonly IShiftStorageService _shiftStorageService;

        public GetChatsCommand(
            IShiftStorageService shiftStorageService)
        {
            _shiftStorageService = shiftStorageService;
        }

        public CommandResult<List<ChatModel>> Execute(
            Guid shiftId,
            Guid? agentId)
        {
            var shift = _shiftStorageService.GetShift(shiftId);

            if (shift == null)
            {
                return new CommandResult<List<ChatModel>>(false, "The shift is not found. ");
            }

            if (shift.IsOverflowAgentsAvailable
                && agentId.HasValue)
            {
                var agent = shift.OverflowAgents.FirstOrDefault(a => a.AgentId == agentId);

                if (agent != null)
                {
                    return new CommandResult<List<ChatModel>>(
                        agent.Chats
                            .Select(a => new ChatModel(a))
                            .ToList());
                }
            }

            if (agentId.HasValue)
            {
                var agent = shift.Agents.FirstOrDefault(a => a.AgentId == agentId);

                if (agent != null)
                {
                    return new CommandResult<List<ChatModel>>(
                        agent.Chats
                            .Select(a => new ChatModel(a))
                            .ToList());
                }
            }

            var chats = shift.Agents.SelectMany(a => a.Chats).ToList();
            chats.AddRange(shift.OverflowAgents.SelectMany(a => a.Chats).ToList());

            return new CommandResult<List<ChatModel>>(
                chats
                    .Select(a => new ChatModel(a))
                    .ToList());
        }
    }
}
