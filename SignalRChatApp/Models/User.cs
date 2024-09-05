namespace SignalRChatApp.Models;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public bool IsOnline { get; set; }
    public DateTime RegisteredAt { get; set; }
    public List<ChatRoom> ChatRooms { get; set; }
    public List<Message> Messages { get; set; }
}