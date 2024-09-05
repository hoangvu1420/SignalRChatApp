using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRChatApp.Hubs;
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
		var claims = new List<Claim>
		{
			new(ClaimTypes.Name, username)
		};

		var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

		await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

		await userService.SetUserOnlineStatus(username, true);
		return Ok(new { username });
	}

	[HttpPost]
	public async Task<IActionResult> Logout()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		await userService.SetUserOnlineStatus(User.Identity.Name, false);
		return Ok();
	}

	[HttpGet]
	public IActionResult CheckAuth()
	{
		bool isAuthenticated = User.Identity?.IsAuthenticated ?? false;
		string username = isAuthenticated ? User.Identity?.Name : null;
		return Json(new { isAuthenticated, username });
	}
}