using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Core.Shifts.Commands;
using ChatAssignmentApp.Domain;
using ChatAssignmentApp.Memory.Services;
using ChatAssignmentApp.Queuing.Services;
using Moq;

namespace ChatAssignmentApp.Core.Test.Shifts
{
    public class EndShiftCommandTest
    {
        [Fact]
        public async Task EndShift_WithQueueItems_Success()
        {
            var config = new Configuration()
            {
                RabbitMQConfiguration = new RabbitMQConfiguration()
                {
                    MainChatQueueName = "abc",
                    OverflowChatQueueName = "def",
                }
            };

            var queueService = new Mock<IQueueService>();
            queueService
                .Setup(a => a.GetQueueItemCount(It.IsAny<string>()))
                .ReturnsAsync((uint)1);

            var agent = new Agent(1, AgentSeniorityType.Junior);
            var shift = new Shift(DateTime.Now, [agent]);

            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.GetShift(It.IsAny<Guid>()))
                .Returns(shift);

            var sut = new EndShiftCommand(
                config,
                queueService.Object,
                shiftStorage.Object);

            var response = await sut.ExecuteAsync(shift.ShiftId);

            Assert.True(response.IsSuccessful);
        }

        [Fact]
        public async Task EndShift_DeleteQueues_Success()
        {
            var config = new Configuration()
            {
                RabbitMQConfiguration = new RabbitMQConfiguration()
                {
                    MainChatQueueName = "abc",
                    OverflowChatQueueName = "def",
                }
            };

            var queueService = new Mock<IQueueService>();
            queueService
                .Setup(a => a.GetQueueItemCount(It.IsAny<string>()))
                .ReturnsAsync((uint)0);

            queueService
                .Setup(a => a.DeleteQueues(It.IsAny<bool>()));

            var agent = new Agent(1, AgentSeniorityType.Junior);
            var shift = new Shift(DateTime.Now, [agent]);

            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.GetShift(It.IsAny<Guid>()))
                .Returns(shift);

            shiftStorage
                .Setup(f => f.DeleteShift(It.IsAny<Shift>()));

            var sut = new EndShiftCommand(
                config,
                queueService.Object,
                shiftStorage.Object);

            var response = await sut.ExecuteAsync(shift.ShiftId);

            Assert.True(response.IsSuccessful);
        }
    }
}
