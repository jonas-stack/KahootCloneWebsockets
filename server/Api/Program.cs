using System.Reflection;
using Api.WebSockets;
using Api.EventHandlers;
using WebSocketBoilerplate;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add necessary services
builder.Services.AddSingleton<IConnectionManager, DictionaryConnectionManager>();
builder.Services.AddSingleton<CustomWebSocketServer>();
builder.Services.AddSingleton<IEventHandlersService, EventHandlersService>();
builder.Services.InjectEventHandlers(Assembly.GetExecutingAssembly());
builder.Services.AddDbContext<KahootDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddLogging();

var app = builder.Build();

try
{

    // Configure middleware and endpoints
    app.UseWebSockets();

    var webSocketServer = app.Services.GetRequiredService<CustomWebSocketServer>();
    webSocketServer.Start(app);

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Exception: {ex.Message}");
}