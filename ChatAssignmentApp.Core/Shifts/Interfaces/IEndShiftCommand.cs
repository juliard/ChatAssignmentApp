using ChatAssignmentApp.Core.Model;

namespace ChatAssignmentApp.Core.Shifts.Interfaces
{
    public interface IEndShiftCommand
    {
        Task<CommandResult<bool>> ExecuteAsync(
            Guid shiftId);
    }
}
