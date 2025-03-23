using ChatAssignmentApp.Domain;

namespace ChatAssignmentApp.Core.Shifts.Models
{
    public class AgentModel
    {
        public Guid AgentId { get; set; }
        public int AgentNumber { get; set; }
        public AgentSeniorityType AgentSeniorityType { get; set; }
        public short MaxChatSessions { get; set; }
        public bool IsOverflowAgent { get; set; }

        public AgentModel(Agent agent)
        {
            AgentId = agent.AgentId;
            AgentNumber = agent.AgentNumber;
            AgentSeniorityType = agent.AgentSeniorityType;
            MaxChatSessions = agent.MaxChatSessions;
            IsOverflowAgent = agent.IsOverflowAgent;
        }
    }
}
