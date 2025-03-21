using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Core.Shifts.Interfaces;
using ChatAssignmentApp.Memory.Services;
using ChatAssignmentApp.Queuing.Services;

namespace ChatAssignmentApp.Core.Shifts.Commands
{
    public class EndShiftCommand : IEndShiftCommand
    {
        private readonly IQueueService _queueService;
        private readonly IShiftStorageService _shiftStorageService;

        public EndShiftCommand(
            IQueueService queueService,
            IShiftStorageService shiftStorageService)
        {
            _queueService = queueService;
            _shiftStorageService = shiftStorageService;
        }

        public async Task<CommandResult<bool>> ExecuteAsync(
            Guid shiftId)
        {
            var currentShift = _shiftStorageService.GetShift(shiftId);

            if (currentShift == null)
                return new CommandResult<bool>(false, "Current shift not found. ");

            await _queueService.DeleteQueues(currentShift.IsOverflowAgentsAvailable);

            _shiftStorageService.DeleteShift(currentShift);

            return new CommandResult<bool>(true);
        }
    }
}
