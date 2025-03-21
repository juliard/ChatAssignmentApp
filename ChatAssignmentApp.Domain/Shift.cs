namespace ChatAssignmentApp.Domain
{
    public class Shift
    {
        public Guid ShiftId { get; set; }
        public DateTime ShiftStart { get; set; }
        public DateTime ShiftEnd { get; set; }
        public bool IsShiftForcefullyEnded { get; set; }
        public bool IsShiftEnded { get => DateTime.UtcNow >= ShiftEnd || IsShiftForcefullyEnded; }

        public List<Agent> Agents { get; set; } = [];
        
        public bool IsOverflowAgentsAvailable { get => OverflowAgents != null && OverflowAgents.Any(); }
        public List<Agent> OverflowAgents { get; set; } = [];

        public int MaxChatsToQueue { get => (int)Math.Floor(Agents.Sum(a => a.MaxChatSessions) * 1.5); }

        public Shift() { }

        public Shift(
            DateTime shiftStart,
            List<Agent> agents)
        {
            ShiftId = Guid.NewGuid();

            ShiftStart = shiftStart;
            ShiftEnd = shiftStart.AddHours(8);
            IsShiftForcefullyEnded = false;

            Agents = agents;
        }

        public void ForceShiftEnd()
        {
            IsShiftForcefullyEnded = true;
        }

        public void AddOverflowAgents(
            List<Agent> overflowAgents)
        {
            OverflowAgents.AddRange(overflowAgents);
        }
    }
}
