using ChatAssignmentApp.Core.Shifts.Interfaces;
using ChatAssignmentApp.Core.Shifts.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatAssignmentApp.Controllers
{
    [Route("api/shifts")]
    public class ShiftController : Controller
    {
        private readonly ICreateShiftCommand _createShiftCommand;
        private readonly IEndShiftCommand _endShiftCommand;
        private readonly IGetShiftsCommand _getShiftsCommand;

        public ShiftController(
            ICreateShiftCommand createShiftCommand,
            IEndShiftCommand endShiftCommand,
            IGetShiftsCommand getShiftsCommand)
        {
            _createShiftCommand = createShiftCommand;
            _endShiftCommand = endShiftCommand;
            _getShiftsCommand = getShiftsCommand;
        }

        [HttpPost]
        public async Task<ActionResult<ShiftModel>> CreateShift(
            [FromBody] CreateShiftModel model)
        {
            var commandResult = await _createShiftCommand.ExecuteAsync(model);

            if (!commandResult.IsSuccessful)
                return BadRequest(commandResult.ErrorMessage);

            return commandResult.Result;
        }

        [HttpGet]
        public ActionResult<List<ShiftModel>> GetShifts()
        {
            var commandResult = _getShiftsCommand.Execute();

            if (!commandResult.IsSuccessful)
                return BadRequest(commandResult.ErrorMessage);

            return commandResult.Result;
        }

        [HttpDelete("{shiftId}")]
        public async Task<ActionResult<bool>> EndShift(
            [FromRoute] Guid shiftId)
        {
            var commandResult = await _endShiftCommand.ExecuteAsync(shiftId);

            if (!commandResult.IsSuccessful)
                return BadRequest(commandResult.ErrorMessage);

            return commandResult.Result;
        }
    }
}
