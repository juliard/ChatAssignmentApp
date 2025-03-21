using ChatAssignmentApp.Domain;

namespace ChatAssignmentApp.Core.Shifts.Models
{
    public class AgentModel
    {
        public Guid AgentId { get; set; }
        public int AgentNumber { get; set; }
        public AgentSeniorityType AgentSeniorityType { get; set; }
        public string AgentQueueName { get; set; } = string.Empty;
        public short MaxChatSessions { get; set; }

        public AgentModel(Agent agent)
        {
            AgentId = agent.AgentId;
            AgentNumber = agent.AgentNumber;
            AgentSeniorityType = agent.AgentSeniorityType;
            AgentQueueName = agent.AgentQueueName;
            MaxChatSessions = agent.MaxChatSessions;
        }
    }
}
