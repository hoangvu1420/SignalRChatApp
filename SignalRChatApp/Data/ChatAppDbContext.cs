using Microsoft.EntityFrameworkCore;
using SignalRChatApp.Models;

namespace SignalRChatApp.Data;

public class ChatAppDbContext(DbContextOptions<ChatAppDbContext> options) : DbContext(options)
{
	public DbSet<User> Users { get; set; }
	public DbSet<ChatRoom> ChatRooms { get; set; }
	public DbSet<Message> Messages { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>()
			.HasMany(u => u.ChatRooms)
			.WithMany(r => r.Users);

		modelBuilder.Entity<User>()
			.HasMany(u => u.Messages)
			.WithOne(m => m.Sender)
			.HasForeignKey(m => m.SenderId);

		modelBuilder.Entity<ChatRoom>()
			.HasMany(r => r.Messages)
			.WithOne(m => m.ChatRoom)
			.HasForeignKey(m => m.ChatRoomId);
	}
}