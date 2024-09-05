using Microsoft.EntityFrameworkCore;
using SignalRChatApp.Data;
using SignalRChatApp.Models;

namespace SignalRChatApp.Services;

public class UserService(ChatAppDbContext context)
{
    public async Task<bool> RegisterUserAsync(string userName)
    {
	    if (string.IsNullOrWhiteSpace(userName))
	    {
			return false;
		}

        // if the user already exists, return the existing user
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        if (existingUser != null)
        {
			return false;
		}

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            IsOnline = true,
            RegisteredAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<User?> GetUserAsync(string username)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
		return await context.Users.ToListAsync();
	}

    public async Task SetUserOnlineStatus(string id, bool isOnline)
    {
		var user = await GetUserAsync(id);
		if (user != null)
		{
			user.IsOnline = isOnline;
			await context.SaveChangesAsync();
		}
	}
}