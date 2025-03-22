using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Core.Shifts.Interfaces;
using ChatAssignmentApp.Memory.Services;
using ChatAssignmentApp.Queuing.Services;

namespace ChatAssignmentApp.Core.Shifts.Commands
{
    public class EndShiftCommand : IEndShiftCommand
    {
        private readonly Configuration _config;

        private readonly IQueueService _queueService;
        private readonly IShiftStorageService _shiftStorageService;

        public EndShiftCommand(
            Configuration configuration,
            IQueueService queueService,
            IShiftStorageService shiftStorageService)
        {
            _config = configuration;
            _queueService = queueService;
            _shiftStorageService = shiftStorageService;
        }

        public async Task<CommandResult<bool>> ExecuteAsync(
            Guid shiftId)
        {
            var currentShift = _shiftStorageService.GetShift(shiftId);

            if (currentShift == null)
                return new CommandResult<bool>(false, "Current shift not found. ");

            var mainQueueItemCount = await _queueService.GetQueueItemCount(
                _config.RabbitMQConfiguration.MainChatQueueName);

            var overflowQueueItemCount = await _queueService.GetQueueItemCount(
                _config.RabbitMQConfiguration.OverflowChatQueueName);

            if (mainQueueItemCount > 0 || overflowQueueItemCount > 0)
            {
                currentShift.ForceShiftEnd();
                return new CommandResult<bool>(true);
            }

            await _queueService.DeleteQueues(currentShift.IsOverflowAgentsAvailable);
            _shiftStorageService.DeleteShift(currentShift);

            return new CommandResult<bool>(true);
        }
    }
}
