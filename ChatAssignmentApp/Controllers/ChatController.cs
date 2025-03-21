using ChatAssignmentApp.Core.Chats.Interfaces;
using ChatAssignmentApp.Core.Chats.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatAssignmentApp.Controllers
{
    [Route("api/shift/{shiftId}/chats")]
    public class ChatController : Controller
    {
        private readonly ICreateChatCommand _createChatCommand;
        private readonly IEndChatCommand _endChatCommand;
        private readonly IGetChatCommand _getChatCommand;
        private readonly IGetChatsCommand _getChatsCommand;

        public ChatController(
            ICreateChatCommand createChatCommand,
            IEndChatCommand endChatCommand,
            IGetChatCommand getChatCommand,
            IGetChatsCommand getChatsCommand)
        {
            _createChatCommand = createChatCommand;
            _endChatCommand = endChatCommand;
            _getChatCommand = getChatCommand;
            _getChatsCommand = getChatsCommand;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> CreateChat(
            [FromRoute] Guid shiftId,
            [FromBody] CreateChatModel model)
        {
            var commandResult = await _createChatCommand.ExecuteAsync(shiftId, model);

            if (!commandResult.IsSuccessful)
                return BadRequest(commandResult);

            return commandResult.Result;
        }

        [HttpDelete("{chatId}")]
        public ActionResult<bool> EndChat(
            [FromRoute] Guid shiftId,
            [FromRoute] Guid chatId)
        {
            var commandResult = _endChatCommand.Execute(shiftId, chatId);

            if (!commandResult.IsSuccessful)
                return BadRequest(commandResult);

            return commandResult.Result;
        }

        [HttpGet]
        public ActionResult<List<ChatModel>> GetChats(
            [FromRoute] Guid shiftId,
            [FromQuery] Guid? agentId)
        {
            var commandResult = _getChatsCommand.Execute(shiftId, agentId);

            if (!commandResult.IsSuccessful)
                return BadRequest(commandResult);

            return commandResult.Result;
        }

        [HttpGet("{chatId}")]
        public ActionResult<ChatModel> GetChat(
            [FromRoute] Guid shiftId,
            [FromRoute] Guid chatId)
        {
            var commandResult = _getChatCommand.Execute(shiftId, chatId);

            if (!commandResult.IsSuccessful)
                return BadRequest(commandResult);

            return commandResult.Result;
        }
    }
}
