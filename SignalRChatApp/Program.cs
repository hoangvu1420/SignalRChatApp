using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SignalRChatApp.Data;
using SignalRChatApp.Hubs;
using SignalRChatApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ChatAppDbContext>(options =>
    options.UseInMemoryDatabase("ChatAppDb"));

builder.Services.AddSignalR(options =>
{
	options.EnableDetailedErrors = true; // Enable detailed errors
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Remove the cookie authentication
// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookie();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ChatRoomService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<DataSeeder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chatHub"); // Map the ChatHub to the /chatHub URL

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await dataSeeder.Seed();
}

app.Run();
