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
builder.Services.AddScoped<AdminStartsGameEventHandler>();
builder.Services.AddDbContext<KahootDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddLogging();

var app = builder.Build();

try
{
    // Ensure that IEventHandlersService is properly initialized
    var eventHandlersService = app.Services.GetRequiredService<IEventHandlersService>();
    if (eventHandlersService.EventHandlers == null)
    {
        throw new InvalidOperationException("EventHandlers cannot be null.");
    }

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