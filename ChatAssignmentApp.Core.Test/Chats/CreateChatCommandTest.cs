using ChatAssignmentApp.Core.Chats.Commands;
using ChatAssignmentApp.Core.Chats.Models;
using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Domain;
using ChatAssignmentApp.Memory.Services;
using ChatAssignmentApp.Queuing.Services;
using Moq;

namespace ChatAssignmentApp.Core.Test.Chats
{
    public class CreateChatCommandTest
    {
        [Fact]
        public async Task CreateChat_Success()
        {
            var request = new CreateChatModel()
            {
                ChatStart = DateTime.Now,
                Message = "Hello world",
            };

            var config = new Configuration()
            {
                RabbitMQConfiguration = new RabbitMQConfiguration()
                {
                    MainChatQueueName = "abc",
                    OverflowChatQueueName = "def",
                }
            };

            var agent = new Agent(1, AgentSeniorityType.Junior, false);
            var shift = new Shift(DateTime.Now, [agent]);
            shift.AddOverflowAgents([agent]);

            var queueService = new Mock<IQueueService>();
            queueService
                .Setup(a => a.GetQueueItemCount(config.RabbitMQConfiguration.MainChatQueueName))
                .ReturnsAsync((uint)4);
            queueService
                .Setup(a => a.GetQueueItemCount(config.RabbitMQConfiguration.OverflowChatQueueName))
                .ReturnsAsync((uint)0);
            queueService
                .Setup(a => a.Enqueue(It.IsAny<string>(), It.IsAny<Chat>()));

            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.GetShift(It.IsAny<Guid>()))
                .Returns(shift);

            var sut = new CreateChatCommand(
                config,
                queueService.Object,
                shiftStorage.Object);

            var response = await sut.ExecuteAsync(Guid.NewGuid(), request);

            Assert.True(response.IsSuccessful);
        }

        [Fact]
        public async Task CreateChat_EnqueueToOverflowQueue_Success()
        {
            var request = new CreateChatModel()
            {
                ChatStart = DateTime.Now,
                Message = "Hello world",
            };

            var config = new Configuration()
            {
                RabbitMQConfiguration = new RabbitMQConfiguration()
                {
                    MainChatQueueName = "abc",
                    OverflowChatQueueName = "def",
                }
            };

            var agent = new Agent(1, AgentSeniorityType.Junior, false);
            var shift = new Shift(DateTime.Now, [agent]);
            shift.AddOverflowAgents([agent]);

            var queueService = new Mock<IQueueService>();
            queueService
                .Setup(a => a.GetQueueItemCount(config.RabbitMQConfiguration.MainChatQueueName))
                .ReturnsAsync((uint)6);
            queueService
                .Setup(a => a.GetQueueItemCount(config.RabbitMQConfiguration.OverflowChatQueueName))
                .ReturnsAsync((uint)0);
            queueService
                .Setup(a => a.Enqueue(It.IsAny<string>(), It.IsAny<Chat>()));

            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.GetShift(It.IsAny<Guid>()))
                .Returns(shift);

            var sut = new CreateChatCommand(
                config,
                queueService.Object,
                shiftStorage.Object);

            var response = await sut.ExecuteAsync(Guid.NewGuid(), request);

            Assert.True(response.IsSuccessful);
        }

        [Fact]
        public async Task CreateChat_ShiftEnded_Failed()
        {
            var request = new CreateChatModel()
            {
                ChatStart = DateTime.Now,
                Message = "Hello world",
            };

            var config = new Configuration()
            {
                RabbitMQConfiguration = new RabbitMQConfiguration()
                {
                    MainChatQueueName = "abc",
                    OverflowChatQueueName = "def",
                }
            };

            var agent = new Agent(1, AgentSeniorityType.Junior, false);
            var shift = new Shift(DateTime.Now, [agent]);
            shift.ForceShiftEnd();

            var queueService = new Mock<IQueueService>();

            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.GetShift(It.IsAny<Guid>()))
                .Returns(shift);

            var sut = new CreateChatCommand(
                config,
                queueService.Object,
                shiftStorage.Object);

            var response = await sut.ExecuteAsync(Guid.NewGuid(), request);

            Assert.False(response.IsSuccessful);
        }

        [Fact]
        public async Task CreateChat_QueuesFull_Failed()
        {
            var request = new CreateChatModel()
            {
                ChatStart = DateTime.Now,
                Message = "Hello world",
            };

            var config = new Configuration()
            {
                RabbitMQConfiguration = new RabbitMQConfiguration()
                {
                    MainChatQueueName = "abc",
                    OverflowChatQueueName = "def",
                }
            };

            var agent = new Agent(1, AgentSeniorityType.Junior, false);
            var shift = new Shift(DateTime.Now, [agent]);
            shift.AddOverflowAgents([agent]);

            var queueService = new Mock<IQueueService>();
            queueService
                .Setup(a => a.GetQueueItemCount(config.RabbitMQConfiguration.MainChatQueueName))
                .ReturnsAsync((uint)6);
            queueService
                .Setup(a => a.GetQueueItemCount(config.RabbitMQConfiguration.OverflowChatQueueName))
                .ReturnsAsync((uint)6);
            queueService
                .Setup(a => a.Enqueue(It.IsAny<string>(), It.IsAny<Chat>()));

            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.GetShift(It.IsAny<Guid>()))
                .Returns(shift);

            var sut = new CreateChatCommand(
                config,
                queueService.Object,
                shiftStorage.Object);

            var response = await sut.ExecuteAsync(Guid.NewGuid(), request);

            Assert.False(response.IsSuccessful);
        }
    }
}
