﻿namespace ChatAssignmentApp.Domain
{
    public class Shift
    {
        public Guid ShiftId { get; set; }
        public DateTime ShiftStart { get; set; }
        public DateTime ShiftEnd { get; set; }
        public bool IsShiftEnded { get => DateTime.UtcNow >= ShiftEnd; }

        public List<Agent> Agents { get; set; } = [];
        
        public bool IsOverflowAgentsAvailable { get => OverflowAgents != null && OverflowAgents.Any(); }
        public List<Agent> OverflowAgents { get; set; } = [];

        public int MaxChatsToQueue { get => Agents.Sum(a => a.MaxChatSessions); }

        public Shift() { }

        public Shift(
            DateTime shiftStart,
            List<Agent> agents)
        {
            ShiftId = Guid.NewGuid();

            ShiftStart = shiftStart;
            ShiftEnd = shiftStart.AddHours(8);

            Agents = agents;
        }

        public void AddOverflowAgents(
            List<Agent> overflowAgents)
        {
            OverflowAgents.AddRange(overflowAgents);
        }
    }
}
