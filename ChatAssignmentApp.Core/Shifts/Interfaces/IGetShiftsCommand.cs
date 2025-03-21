using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Core.Shifts.Models;

namespace ChatAssignmentApp.Core.Shifts.Interfaces
{
    public interface IGetShiftsCommand
    {
        CommandResult<List<ShiftModel>> Execute();
    }
}
