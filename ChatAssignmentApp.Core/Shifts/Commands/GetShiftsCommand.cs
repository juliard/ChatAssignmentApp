using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Core.Shifts.Interfaces;
using ChatAssignmentApp.Core.Shifts.Models;
using ChatAssignmentApp.Memory.Services;

namespace ChatAssignmentApp.Core.Shifts.Commands
{
    public class GetShiftsCommand : IGetShiftsCommand
    {
        private readonly IShiftStorageService _shiftStorageService;

        public GetShiftsCommand(
            IShiftStorageService shiftStorageService)
        {
            _shiftStorageService = shiftStorageService;
        }

        public CommandResult<List<ShiftModel>> Execute()
        {
            var shifts = _shiftStorageService.GetShifts();

            if (shifts == null)
                return new CommandResult<List<ShiftModel>>(false, "There are no shifts found. ");

            return new CommandResult<List<ShiftModel>>(
                shifts
                    .Select(a => new ShiftModel(a))
                    .ToList());
        }
    }
}
