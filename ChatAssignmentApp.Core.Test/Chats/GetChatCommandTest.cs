using ChatAssignmentApp.Core.Chats.Commands;
using ChatAssignmentApp.Domain;
using ChatAssignmentApp.Memory.Services;
using Moq;

namespace ChatAssignmentApp.Core.Test.Chats
{
    public class GetChatCommandTest
    {
        [Fact]
        public void GetChat_Success()
        {
            var chat = new Chat(DateTime.Now, "Hello world");

            var agent = new Agent(1, AgentSeniorityType.Junior);
            agent.AddChat(chat);

            var shift = new Shift(DateTime.Now, [agent]);

            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.GetShift(It.IsAny<Guid>()))
                .Returns(shift);

            var sut = new GetChatCommand(shiftStorage.Object);

            var response = sut.Execute(Guid.NewGuid(), chat.ChatId);

            Assert.True(response.IsSuccessful);
        }

        [Fact]
        public void GetChat_ChatNotFound()
        {
            var agent = new Agent(1, AgentSeniorityType.Junior);
            var shift = new Shift(DateTime.Now, [agent]);

            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.GetShift(It.IsAny<Guid>()))
                .Returns(shift);

            var sut = new GetChatCommand(shiftStorage.Object);

            var response = sut.Execute(Guid.NewGuid(), Guid.NewGuid());

            Assert.False(response.IsSuccessful);
        }
    }
}
