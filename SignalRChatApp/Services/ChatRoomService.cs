using Microsoft.EntityFrameworkCore;
using SignalRChatApp.Data;
using SignalRChatApp.Models;

namespace SignalRChatApp.Services;

public class ChatRoomService(ChatAppDbContext dbContext)
{
	public async Task CreateChatRoomAsync(string name, bool isPrivate)
	{
		if (await ChatRoomExistsAsync(name))
		{
			throw new InvalidOperationException($"A chat room with the name '{name}' already exists.");
		}

		var chatRoom = new ChatRoom
		{
			Name = name,
			IsPrivate = isPrivate
		};

		await dbContext.ChatRooms.AddAsync(chatRoom);
		await dbContext.SaveChangesAsync();
	}

	public async Task<bool> ChatRoomExistsAsync(string name)
	{
		return await dbContext.ChatRooms.AnyAsync(cr => cr.Name == name);
	}

	public async Task<IEnumerable<ChatRoomDto>> GetAllChatRoomsAsync()
	{
		return await dbContext.ChatRooms
			.Select(cr => new ChatRoomDto
			{
				Id = cr.Id,
				Name = cr.Name,
				MessageCount = cr.Messages.Count,
				LastMessageTime = cr.Messages.Any() ? cr.Messages.Max(m => m.Timestamp) : DateTime.MinValue
			})
			.ToListAsync();
	}

	public async Task<ChatRoomDto?> GetChatRoomByNameAsync(string name)
	{
		return await dbContext.ChatRooms
			.Where(cr => cr.Name == name)
			.Select(cr => new ChatRoomDto
			{
				Id = cr.Id,
				Name = cr.Name,
				MessageCount = cr.Messages.Count,
				LastMessageTime = cr.Messages.Any() ? cr.Messages.Max(m => m.Timestamp) : DateTime.MinValue
			})
			.FirstOrDefaultAsync();
	}

	public async Task AddUserToChatRoomAsync(Guid chatRoomId, Guid userId)
	{
		var chatRoom = await dbContext.ChatRooms.Include(cr => cr.Users).FirstOrDefaultAsync(cr => cr.Id == chatRoomId);
		var user = await dbContext.Users.FindAsync(userId);

		if (chatRoom == null || user == null) return;

		if (chatRoom.Users.Contains(user))
		{
			return;
		}

		chatRoom.Users.Add(user);
		await dbContext.SaveChangesAsync();
	}

	public async Task RemoveUserFromChatRoomAsync(Guid chatRoomId, Guid userId)
	{
		var chatRoom = await dbContext.ChatRooms.FindAsync(chatRoomId);
		var user = await dbContext.Users.FindAsync(userId);

		if (chatRoom == null || user == null) return;

		chatRoom.Users.Remove(user);
		await dbContext.SaveChangesAsync();
	}

	public async Task<IEnumerable<MessageDto>> GetChatRoomMessagesAsync(string chatRoomName)
	{
		return await dbContext.Messages
			.Where(m => m.ChatRoom.Name == chatRoomName)
			.Select(m => new MessageDto
			{
				User = m.Sender.UserName,
				Message = m.Content,
				Timestamp = m.Timestamp,
				RoomName = chatRoomName
			})
			.ToListAsync();
	}

	public async Task<IEnumerable<ChatRoom>?> GetChatRoomsForUserAsync(Guid userId)
	{
		var user = await dbContext.Users
			.Include(u => u.ChatRooms)
			.FirstOrDefaultAsync(u => u.Id == userId);
		return user?.ChatRooms;
	}
}