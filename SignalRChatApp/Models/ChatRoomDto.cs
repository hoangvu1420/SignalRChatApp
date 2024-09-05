namespace SignalRChatApp.Models;

public class ChatRoomDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int MessageCount { get; set; }
    public DateTime LastMessageTime { get; set; }
}