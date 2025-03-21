namespace ChatAssignmentApp.Domain
{
    public class Chat
    {
        public Guid ChatId { get; set; }
        public DateTime ChatStart { get; set; }
        public string Message { get; set; } = string.Empty;

        public Chat(
            DateTime chatStart,
            string message)
        {
            ChatId = Guid.NewGuid();
            ChatStart = chatStart;
            Message = message;
        }
    }
}
