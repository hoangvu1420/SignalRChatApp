using Microsoft.AspNetCore.Mvc;
using SignalRChatApp.Services;

namespace SignalRChatApp.Controllers;

public class GroupChatController(ChatRoomService chatRoomService) : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet]
    [Route("GroupChat/GetGroupChats")]
    public async Task<IActionResult> GetGroupChats()
    {
        var groupChats = await chatRoomService.GetAllChatRoomsAsync();
        return Ok(groupChats);
    }

    [HttpGet]
    [Route("GroupChat/GetGroupChat")]
    public async Task<IActionResult> GetGroupChat(string name)
    {
        var groupChat = await chatRoomService.GetChatRoomByNameAsync(name);
        return Ok(groupChat);
    }

    [HttpGet]
    [Route("GroupChat/GetGroupChatMessages")]
    public async Task<IActionResult> GetGroupChatMessages(string name)
    {
        var groupChatMessages = await chatRoomService.GetChatRoomMessagesAsync(name);
        return Ok(groupChatMessages);
    }
}