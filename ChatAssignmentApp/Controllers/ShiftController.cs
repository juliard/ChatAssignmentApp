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

        public ShiftController(
            ICreateShiftCommand createShiftCommand,
            IEndShiftCommand endShiftCommand)
        {
            _createShiftCommand = createShiftCommand;
            _endShiftCommand = endShiftCommand;
        }

        [HttpPost]
        public async Task<ActionResult<ShiftModel>> CreateShift(
            [FromBody] CreateShiftModel model)
        {
            var commandResult = await _createShiftCommand.ExecuteAsync(model);

            if (!commandResult.IsSuccessful)
                return BadRequest(commandResult);

            return commandResult.Result;
        }

        [HttpDelete("{shiftId}")]
        public async Task<ActionResult<bool>> EndShift(
            [FromRoute] Guid shiftId)
        {
            var commandResult = await _endShiftCommand.ExecuteAsync(shiftId);

            if (!commandResult.IsSuccessful)
                return BadRequest(commandResult);

            return commandResult.Result;
        }
    }
}
