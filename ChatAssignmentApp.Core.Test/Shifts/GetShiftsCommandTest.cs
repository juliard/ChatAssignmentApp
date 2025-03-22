using ChatAssignmentApp.Core.Shifts.Commands;
using ChatAssignmentApp.Domain;
using ChatAssignmentApp.Memory.Services;
using Moq;

namespace ChatAssignmentApp.Core.Test.Shifts
{
    public class GetShiftsCommandTest
    {
        [Fact]
        public void GetShifts_Success()
        {
            var agent = new Agent(1, AgentSeniorityType.Junior);
            var shift = new Shift(DateTime.Now, [agent]);

            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.GetShifts())
                .Returns([shift]);

            var sut = new GetShiftsCommand(shiftStorage.Object);

            var response = sut.Execute();

            Assert.True(response.IsSuccessful);
        }

        [Fact]
        public void GetShifts_NoShiftsFound()
        {
            var shiftStorage = new Mock<IShiftStorageService>();
            shiftStorage
                .Setup(f => f.GetShifts())
                .Returns((List<Shift>)null);

            var sut = new GetShiftsCommand(shiftStorage.Object);

            var response = sut.Execute();

            Assert.False(response.IsSuccessful);
        }
    }
}
