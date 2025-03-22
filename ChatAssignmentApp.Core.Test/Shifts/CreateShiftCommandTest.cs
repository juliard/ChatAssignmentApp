using ChatAssignmentApp.Core.Model;
using ChatAssignmentApp.Core.Shifts.Commands;
using ChatAssignmentApp.Core.Shifts.Models;
using ChatAssignmentApp.Domain;
using ChatAssignmentApp.Memory.Services;
using ChatAssignmentApp.Queuing.Services;
using Moq;

namespace ChatAssignmentApp.Core.Test.Shifts
{
    public class CreateShiftCommandTest
    {
        [Fact]
        public async Task CreateShift_Success()
        {
            var request = new CreateShiftModel()
            {
                ShiftStart = DateTime.Now,
                NumberOfJuniorAgents = 1,
                NumberOfMidAgents = 1,
                NumberOfSeniorAgents = 1,
                NumberOfLeadAgents = 1,
                IsOverflowAgentsAvailable = true,
            };

            var queueService = new Mock<IQueueService>();
            queueService
                .Setup(a => a.CreateQueues(It.IsAny<int>(), It.IsAny<bool>()));

            var agent = new Agent(1, AgentSeniorityType.Junior);
            var shift = new Shift(DateTime.Now, [agent]);

            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.DoesShiftExist())
                .Returns(false);

            shiftStorage
                .Setup(f => f.CreateShift(It.IsAny<Shift>()));

            var sut = new CreateShiftCommand(
                queueService.Object,
                shiftStorage.Object);

            var response = await sut.ExecuteAsync(request);

            Assert.True(response.IsSuccessful);
        }

        [Fact]
        public async Task CreateShift_NoAgents_Failed()
        {
            var request = new CreateShiftModel()
            {
                ShiftStart = DateTime.Now,
                NumberOfJuniorAgents = 0,
                NumberOfMidAgents = 0,
                NumberOfSeniorAgents = 0,
                NumberOfLeadAgents = 0,
                IsOverflowAgentsAvailable = true,
            };

            var queueService = new Mock<IQueueService>();
            var shiftStorage = new Mock<IShiftStorageService>();

            var sut = new CreateShiftCommand(
                queueService.Object,
                shiftStorage.Object);

            var response = await sut.ExecuteAsync(request);

            Assert.False(response.IsSuccessful);
        }

        [Fact]
        public async Task CreateShift_ShiftExists_Failed()
        {
            var request = new CreateShiftModel()
            {
                ShiftStart = DateTime.Now,
                NumberOfJuniorAgents = 1,
                NumberOfMidAgents = 0,
                NumberOfSeniorAgents = 0,
                NumberOfLeadAgents = 0,
                IsOverflowAgentsAvailable = true,
            };

            var queueService = new Mock<IQueueService>();
            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.DoesShiftExist())
                .Returns(true);

            var sut = new CreateShiftCommand(
                queueService.Object,
                shiftStorage.Object);

            var response = await sut.ExecuteAsync(request);

            Assert.False(response.IsSuccessful);
        }
    }
}
