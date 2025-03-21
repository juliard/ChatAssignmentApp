namespace ChatAssignmentApp.Core.Chats.Models
{
    public class CreateChatModel
    {
        public DateTime ChatStart { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
