namespace ChatAssignmentApp.Domain
{
    public class Agent
    {
        public Guid AgentId { get; set; }
        public int AgentNumber { get; set; }
        public AgentSeniorityType AgentSeniorityType { get; set; }
        public short MaxChatSessions { get; set; }
        public bool IsOverflowAgent { get; set; }

        public List<Chat> Chats { get; set; } = [];

        public Agent() { }

        public Agent(
            int agentNumber,
            AgentSeniorityType agentSeniorityType,
            bool isOverflowAgent)
        {
            AgentId = Guid.NewGuid();
            AgentNumber = agentNumber;
            AgentSeniorityType = agentSeniorityType;
            MaxChatSessions = AgentMaxChatSessionCount.Get(agentSeniorityType);
            IsOverflowAgent = isOverflowAgent;
        }

        public void AddChat(Chat chat)
        {
            if (Chats.Count == MaxChatSessions)
                return;

            Chats.Add(chat);
        }
    }
}
