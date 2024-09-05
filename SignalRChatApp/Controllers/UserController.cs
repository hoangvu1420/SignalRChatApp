using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SignalRChatApp.Services;

namespace SignalRChatApp.Controllers;

public class UserController(UserService userService)
	: Controller
{

	[HttpPost]
	public async Task<IActionResult> Register(string username)
	{
		if (string.IsNullOrWhiteSpace(username))
		{
			return BadRequest("Username cannot be empty.");
		}

		if (await userService.RegisterUserAsync(username))
		{
			return Ok();
		}

		return BadRequest("Username already exists.");
	}

	[HttpPost]
	public async Task<IActionResult> Login(string username)
	{
		var user = await userService.GetUserAsync(username);
		if (user == null) return NotFound("User not found.");

		await userService.SetUserOnlineStatus(username, true);
		return Ok(new { username });
	}

	[HttpPost]
	public async Task<IActionResult> Logout(string username)
	{
		if (!string.IsNullOrEmpty(username))
		{
			await userService.SetUserOnlineStatus(username, false);
		}
		return Ok();
	}
}