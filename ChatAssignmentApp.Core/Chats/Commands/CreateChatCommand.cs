using ChatAssignmentApp.Core.Chats.Interfaces;
using ChatAssignmentApp.Core.Chats.Models;
using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Domain;
using ChatAssignmentApp.Memory.Services;
using ChatAssignmentApp.Queuing.Services;

namespace ChatAssignmentApp.Core.Chats.Commands
{
    public class CreateChatCommand : ICreateChatCommand
    {
        private readonly Configuration _config;
        private readonly IQueueService _queueService;
        private readonly IShiftStorageService _shiftStorageService;

        public CreateChatCommand(
            Configuration configuration,
            IQueueService queueService,
            IShiftStorageService shiftStorageService)
        {
            _config = configuration;
            _queueService = queueService;
            _shiftStorageService = shiftStorageService;
        }

        public async Task<CommandResult<bool>> ExecuteAsync(
            Guid? shiftId,
            CreateChatModel model)
        {
            if (!shiftId.HasValue)
                return new CommandResult<bool>(false, "Current shift not found. ");

            var shift = _shiftStorageService.GetShift(shiftId.Value);

            if (shift == null)
                return new CommandResult<bool>(false, "Current shift not found. ");

            if (DateTime.UtcNow > shift.ShiftEnd)
                return new CommandResult<bool>(false, "Current shift has ended. Please wait for the next shift. ");

            var chat = new Chat(
                model.ChatStart,
                model.Message);

            await _queueService.Enqueue(
                _config.RabbitMQConfiguration.MainChatQueueName,
                chat);

            if (shift.IsOverflowAgentsAvailable)
            {
                await _queueService.Enqueue(
                    _config.RabbitMQConfiguration.OverflowChatQueueName,
                    chat);
            }

            return new CommandResult<bool>(true);
        }
    }
}
