using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalRChatApp.Models;
using SignalRChatApp.Services;

namespace SignalRChatApp.Hubs;

public class ChatHub(
    UserService userService,
    ChatRoomService chatRoomService,
    MessageService messageService,
    ILogger<ChatHub> logger) : Hub
{
    public async Task SendMessage(string roomName, string username, string message)
    {
        logger.LogInformation("Attempting to send message. Room: {RoomName}, User: {Username}", roomName, username);
        
        if (await chatRoomService.ChatRoomExistsAsync(roomName))
        {
            var user = await userService.GetUserAsync(username);
            if (user != null)
            {
                var chatMessage = new MessageDto
                {
                    User = username,
                    Message = message,
                    Timestamp = DateTime.Now,
                    RoomName = roomName
                };
                
                var group = Clients.Groups(roomName);

                if (roomName.Equals("Global", StringComparison.OrdinalIgnoreCase))
                {
                    await Clients.All.SendAsync("ReceiveMessage", chatMessage);
                    logger.LogInformation("Global message broadcasted to all clients");
                }
                else
                {
                    await Clients.Group(roomName).SendAsync("ReceiveMessage", chatMessage);
                    logger.LogInformation("Message sent to room: {RoomName}", roomName);
                }

                var room = await chatRoomService.GetChatRoomByNameAsync(roomName);
                if (room != null)
                {
                    await messageService.CreateMessageAsync(message, user.Id, room.Id);
                    logger.LogInformation("Message saved to database. Room: {RoomName}, User: {Username}", roomName, username);
                }
            }
            else
            {
                logger.LogWarning("User not found. Username: {Username}", username);
            }
        }
        else
        {
            logger.LogWarning("Room does not exist. Room: {RoomName}", roomName);
            throw new HubException("Room does not exist.");
        }
    }

    public async Task JoinRoom(string roomName, string username)
    {
        logger.LogInformation("User attempting to join room. Room: {RoomName}, User: {Username}", roomName, username);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        var user = await userService.GetUserAsync(username);
        var room = await chatRoomService.GetChatRoomByNameAsync(roomName);
        if (user != null && room != null)
        {
            await chatRoomService.AddUserToChatRoomAsync(room.Id, user.Id);
            await Clients.Group(roomName).SendAsync("UserJoinedRoom", roomName, username);
            logger.LogInformation("User joined room successfully. Room: {RoomName}, User: {Username}", roomName, username);
        }
        else
        {
            logger.LogWarning("Failed to join room. User or room not found. Room: {RoomName}, User: {Username}", roomName, username);
        }
    }

    public async Task CreateChatRoom(string roomName)
    {
        logger.LogInformation("Attempting to create chat room. Room: {RoomName}", roomName);
        
        await chatRoomService.CreateChatRoomAsync(roomName, false);
        var updatedRooms = await chatRoomService.GetAllChatRoomsAsync();
        await Clients.All.SendAsync("UpdateGroupList", updatedRooms);
        logger.LogInformation("Chat room created successfully. Room: {RoomName}", roomName);
    }

    public async Task UserConnected(string username)
    {
        logger.LogInformation("User connected. User: {Username}", username);
        
        await userService.SetUserOnlineStatus(username, true);
        await Groups.AddToGroupAsync(Context.ConnectionId, "Global");
        await Clients.All.SendAsync("UserConnected", username);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("User disconnected. ConnectionId: {ConnectionId}", Context.ConnectionId);
        
        var users = await userService.GetAllUsersAsync();
        var user = users.FirstOrDefault(u => u.IsOnline);
        if (user != null)
        {
            await userService.SetUserOnlineStatus(user.UserName, false);
            await Clients.All.SendAsync("UserDisconnected", user.UserName);

            var userRooms = await chatRoomService.GetChatRoomsForUserAsync(user.Id);
            if (userRooms != null)
            {
                foreach (var room in userRooms)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.Name);
                    logger.LogInformation("User removed from room. User: {Username}, Room: {RoomName}", user.UserName, room.Name);
                }
            }
        }

        if (exception != null)
        {
            logger.LogError(exception, "Error occurred during disconnection. ConnectionId: {ConnectionId}", Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}