using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRChatApp.Models;

public class Message
{
	public Guid Id { get; set; }
	[Required]
	public string Content { get; set; } = null!;
	public DateTime Timestamp { get; set; }
	public Guid SenderId { get; set; }
	public User Sender { get; set; }
	public Guid ChatRoomId { get; set; }
	public ChatRoom ChatRoom { get; set; }
}