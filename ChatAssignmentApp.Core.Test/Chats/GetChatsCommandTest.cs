using ChatAssignmentApp.Core.Chats.Commands;
using ChatAssignmentApp.Domain;
using ChatAssignmentApp.Memory.Services;
using Moq;

namespace ChatAssignmentApp.Core.Test.Chats
{
    public class GetChatsCommandTest
    {
        [Fact]
        public void GetChats_Success()
        {
            var chat = new Chat(DateTime.Now, "Hello world");

            var agent = new Agent(1, AgentSeniorityType.Junior, false);
            agent.AddChat(chat);

            var shift = new Shift(DateTime.Now, [agent]);
            shift.AddOverflowAgents([agent]);

            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.GetShift(It.IsAny<Guid>()))
                .Returns(shift);

            var sut = new GetChatsCommand(shiftStorage.Object);

            var response = sut.Execute(shift.ShiftId, null);

            Assert.True(response.Result.Count == 2);
        }

        [Fact]
        public void GetChats_ByAgent_Success()
        {
            var chat = new Chat(DateTime.Now, "Hello world");

            var agent = new Agent(1, AgentSeniorityType.Junior, false);
            agent.AddChat(chat);

            var agent2 = new Agent(1, AgentSeniorityType.Junior, true);
            agent2.AddChat(chat);

            var shift = new Shift(DateTime.Now, [agent]);
            shift.AddOverflowAgents([agent2]);

            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.GetShift(It.IsAny<Guid>()))
                .Returns(shift);

            var sut = new GetChatsCommand(shiftStorage.Object);

            var response = sut.Execute(shift.ShiftId, agent.AgentId);

            Assert.True(response.Result.Count == 1);
        }
    }
}
