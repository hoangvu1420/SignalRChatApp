namespace SignalRChatApp.Models;

public class MessageDto
{
    public string User { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public string RoomName { get; set; } = null!;
}