using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Core.Shifts.Models;

namespace ChatAssignmentApp.Core.Shifts.Interfaces
{
    public interface ICreateShiftCommand
    {
        Task<CommandResult<ShiftModel>> Execute(
            CreateShiftModel model);
    }
}
