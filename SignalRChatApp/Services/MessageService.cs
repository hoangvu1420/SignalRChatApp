using Microsoft.EntityFrameworkCore;
using SignalRChatApp.Data;
using SignalRChatApp.Models;

namespace SignalRChatApp.Services;

public class MessageService(ChatAppDbContext dbContext)
{
    public async Task CreateMessageAsync(string content, Guid senderId, Guid chatRoomId)
    {
        var message = new Message
        {
            Content = content,
            Timestamp = DateTime.UtcNow,
            SenderId = senderId,
            ChatRoomId = chatRoomId
        };

        await dbContext.Messages.AddAsync(message);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Message>?> GetMessagesForChatRoomAsync(Guid chatRoomId)
    {
        return await dbContext.Messages
                            .Include(m => m.Sender)
                            .Where(m => m.ChatRoomId == chatRoomId)
                            .ToListAsync();
    }
}