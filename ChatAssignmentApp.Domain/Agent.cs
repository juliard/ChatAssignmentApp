namespace ChatAssignmentApp.Domain
{
    public class Agent
    {
        public Guid AgentId { get; set; }
        public int AgentNumber { get; set; }
        public AgentSeniorityType AgentSeniorityType { get; set; }
        public string AgentQueueName { get; set; } = string.Empty;
        public short MaxChatSessions { get; set; }

        public List<Chat> Chats { get; set; } = [];

        public Agent(
            int agentNumber,
            AgentSeniorityType agentSeniorityType)
        {
            AgentId = Guid.NewGuid();
            AgentNumber = agentNumber;
            AgentSeniorityType = agentSeniorityType;
            AgentQueueName = $"chat-queue-{agentSeniorityType}-{agentNumber}";
            MaxChatSessions = AgentMaxChatSessionCount.Get(agentSeniorityType);
        }

        public void AddChat(Chat chat)
        {
            if (Chats.Count == MaxChatSessions)
                return;

            Chats.Add(chat);
        }
    }
}
