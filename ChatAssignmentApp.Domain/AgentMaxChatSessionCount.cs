namespace ChatAssignmentApp.Domain
{
    public static class AgentMaxChatSessionCount
    {
        private static short Junior = 4;
        private static short Mid = 6;
        private static short Senior = 8;
        private static short Lead = 5;

        public static short Get(
            AgentSeniorityType agentSeniorityType)
        {
            return agentSeniorityType switch
            {
                AgentSeniorityType.Junior => Junior,
                AgentSeniorityType.Mid => Mid,
                AgentSeniorityType.Senior => Senior,
                AgentSeniorityType.Lead => Lead,
                _ => Junior,
            };
        }
    }
}
