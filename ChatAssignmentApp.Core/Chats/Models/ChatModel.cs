using ChatAssignmentApp.Domain;

namespace ChatAssignmentApp.Core.Chats.Models
{
    public class ChatModel
    {
        public Guid ChatId { get; set; }

        public Guid? AgentId { get; set; }

        public DateTime ChatStart { get; set; }
        public DateTime ChatLastModified { get; set; }
        public string Message { get; set; } = string.Empty;

        public ChatModel(
            Chat chat)
        {
            ChatId = chat.ChatId;
            AgentId = chat.ChatId;

            ChatStart = chat.ChatStart;
            ChatLastModified = chat.ChatLastModified;
            Message = chat.Message;
        }
    }
}
