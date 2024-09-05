using System.ComponentModel.DataAnnotations;

namespace SignalRChatApp.Models;

public class ChatRoom
{
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; } = null!;
	public bool IsPrivate { get; set; }
	public List<User> Users { get; set; } = null!;
	public List<Message> Messages { get; set; }
}