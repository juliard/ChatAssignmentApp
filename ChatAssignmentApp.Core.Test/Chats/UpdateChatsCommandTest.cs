using ChatAssignmentApp.Core.Chats.Commands;
using ChatAssignmentApp.Domain;
using ChatAssignmentApp.Memory.Services;
using Moq;

namespace ChatAssignmentApp.Core.Test.Chats
{
    public class UpdateChatsCommandTest
    {
        [Fact]
        public void UpdateChat_Success()
        {
            var chat = new Chat(DateTime.Now, "Hello world");
            var chat2 = new Chat(DateTime.Now, "Hello world");

            var agent = new Agent(1, AgentSeniorityType.Junior);
            agent.AddChat(chat);
            agent.AddChat(chat2);

            var shift = new Shift(DateTime.Now, [agent]);

            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.GetShift(It.IsAny<Guid>()))
                .Returns(shift);

            var sut = new UpdateChatCommand(shiftStorage.Object);

            var response = sut.Execute(shift.ShiftId, chat.ChatId);

            Assert.True(response.IsSuccessful);
        }

        [Fact]
        public void UpdateChat_OverflowAgent_Success()
        {
            var chat = new Chat(DateTime.Now, "Hello world");
            var chat2 = new Chat(DateTime.Now, "Hello world");

            var agent = new Agent(1, AgentSeniorityType.Junior);
            agent.AddChat(chat);

            var overflowAgent = new Agent(1, AgentSeniorityType.Junior);
            overflowAgent.AddChat(chat2);

            var shift = new Shift(DateTime.Now, [agent]);
            shift.AddOverflowAgents([overflowAgent]);

            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.GetShift(It.IsAny<Guid>()))
                .Returns(shift);

            var sut = new UpdateChatCommand(shiftStorage.Object);

            var response = sut.Execute(shift.ShiftId, chat2.ChatId);

            Assert.True(response.IsSuccessful);
        }
    }
}
