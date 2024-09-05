using SignalRChatApp.Models;
using SignalRChatApp.Data;
using Microsoft.EntityFrameworkCore;

namespace SignalRChatApp.Data;

public class DataSeeder
{
    private readonly ChatAppDbContext _context;

    public DataSeeder(ChatAppDbContext context)
    {
        _context = context;
    }

    public async Task Seed()
    {
        // Seed users
        var user1 = await _context.Users.FirstOrDefaultAsync(u => u.UserName == "user1");
        if (user1 == null)
        {
            user1 = new User
            {
                Id = Guid.NewGuid(),
                UserName = "user1",
                IsOnline = false,
                RegisteredAt = DateTime.UtcNow
            };
            _context.Users.Add(user1);
        }

        var user2 = await _context.Users.FirstOrDefaultAsync(u => u.UserName == "user2");
        if (user2 == null)
        {
            user2 = new User
            {
                Id = Guid.NewGuid(),
                UserName = "user2",
                IsOnline = false,
                RegisteredAt = DateTime.UtcNow
            };
            _context.Users.Add(user2);
        }

        var user3 = await _context.Users.FirstOrDefaultAsync(u => u.UserName == "user3");
        if (user3 == null)
        {
            user3 = new User
            {
                Id = Guid.NewGuid(),
                UserName = "user3",
                IsOnline = false,
                RegisteredAt = DateTime.UtcNow
            };
            _context.Users.Add(user3);
        }

        var user4 = await _context.Users.FirstOrDefaultAsync(u => u.UserName == "user4");
        if (user4 == null)
        {
            user4 = new User
            {
                Id = Guid.NewGuid(),
                UserName = "user4",
                IsOnline = false,
                RegisteredAt = DateTime.UtcNow
            };
            _context.Users.Add(user4);
        }

        var user5 = await _context.Users.FirstOrDefaultAsync(u => u.UserName == "user5");
        if (user5 == null)
        {
            user5 = new User
            {
                Id = Guid.NewGuid(),
                UserName = "user5",
                IsOnline = false,
                RegisteredAt = DateTime.UtcNow
            };
            _context.Users.Add(user5);
        }

        // Seed chat room
        var chatRoom = await _context.ChatRooms
            .Include(cr => cr.Users)
            .FirstOrDefaultAsync(c => c.Name == "Global");
        if (chatRoom == null)
        {
            chatRoom = new ChatRoom
            {
                Id = Guid.NewGuid(),
                Name = "Global",
                Users = new List<User>()
            };
            _context.ChatRooms.Add(chatRoom);
        }

        var chatRoom2 = await _context.ChatRooms
            .Include(cr => cr.Users)
            .FirstOrDefaultAsync(c => c.Name == "Group 1");
        if (chatRoom2 == null)
        {
            chatRoom2 = new ChatRoom
            {
                Id = Guid.NewGuid(),
                Name = "Group 1",
                Users = new List<User>()
            };
            _context.ChatRooms.Add(chatRoom2);
        }

        var chatRoom3 = await _context.ChatRooms
            .Include(cr => cr.Users)
            .FirstOrDefaultAsync(c => c.Name == "Group 2");
        if (chatRoom3 == null)
        {
            chatRoom3 = new ChatRoom
            {
                Id = Guid.NewGuid(),
                Name = "Group 2",
                Users = new List<User>()
            };
            _context.ChatRooms.Add(chatRoom3);
        }

        // Add users to chat room if not already added
        if (!chatRoom.Users.Contains(user1))
        {
            chatRoom.Users.Add(user1);
        }
        if (!chatRoom.Users.Contains(user2))
        {
            chatRoom.Users.Add(user2);
        }

        await _context.SaveChangesAsync();
    }
}