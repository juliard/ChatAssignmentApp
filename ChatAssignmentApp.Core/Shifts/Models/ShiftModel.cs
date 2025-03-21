using ChatAssignmentApp.Domain;

namespace ChatAssignmentApp.Core.Shifts.Models
{
    public class ShiftModel
    {
        public Guid ShiftId { get; set; }
        public DateTime ShiftStart { get; set; }
        public DateTime ShiftEnd { get; set; }

        public List<AgentModel> Agents { get; set; } = [];

        public bool IsOverflowAgentsAvailable { get; set; }
        public List<AgentModel> OverflowAgents { get; set; } = [];

        public ShiftModel(
            Shift shift)
        {
            ShiftId = shift.ShiftId;
            ShiftStart = shift.ShiftStart;
            ShiftEnd = shift.ShiftEnd;

            Agents = shift.Agents.Select(a => new AgentModel(a)).ToList();

            IsOverflowAgentsAvailable = shift.IsOverflowAgentsAvailable;
            OverflowAgents = shift.OverflowAgents.Select(a => new AgentModel(a)).ToList();
        }
    }
}
