namespace ChatAssignmentApp.Domain
{
    public class Chat
    {
        public Guid ChatId { get; set; }
        public DateTime ChatStart { get; set; }
        public DateTime ChatLastModified { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsChatActive { get; set; }

        public Chat(
            DateTime chatStart,
            string message)
        {
            ChatId = Guid.NewGuid();
            ChatStart = chatStart;
            ChatLastModified = chatStart;
            Message = message;
            IsChatActive = true;
        }

        public void UpdateChatLastModifiedTime()
        {
            ChatLastModified = DateTime.UtcNow;
        }

        public void SetChatToInactive()
        {
            IsChatActive = false;
        }
    }
}
