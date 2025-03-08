using System.Reflection;
using Api.WebSockets;
using Api.EventHandlers;
using Api.Services;
using DataAccess.ModelDtos;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using WebSocketBoilerplate;

var builder = WebApplication.CreateBuilder(args);

// Register services.
builder.Services.AddSingleton<IConnectionManager, DictionaryConnectionManager>();
builder.Services.AddSingleton<CustomWebSocketServer>();
builder.Services.AddSingleton<IEventHandlersService, EventHandlersService>();
builder.Services.AddScoped<QuestionBroadcastService>();

// ✅ Scan all assemblies (API + DataAccess) to detect all event handlers.
var executingAssembly = Assembly.GetExecutingAssembly();

Console.WriteLine("📌 Scanning Assemblies for Event Handlers:");
foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
{
    Console.WriteLine($"- {asm.FullName}");
}

builder.Services.InjectEventHandlers(executingAssembly);


// Register DbContext.
builder.Services.AddDbContext<KahootDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddLogging();

var app = builder.Build();

// ✅ Use WebSockets
app.UseWebSockets();

// ✅ Debugging: Print all registered event handlers before starting WebSocket server
var eventHandlersService = app.Services.GetRequiredService<IEventHandlersService>();

if (eventHandlersService.EventHandlers.Count == 0)
{
    Console.WriteLine("🚨 No WebSocket Event Handlers were registered! Check `InjectEventHandlers()`.");
}
else
{
    Console.WriteLine("📌 Registered WebSocket Event Handlers:");
    foreach (var handler in eventHandlersService.EventHandlers)
    {
        Console.WriteLine($"- {handler.Name}");
    }
}

// ✅ Start WebSocket Server
var webSocketServer = app.Services.GetRequiredService<CustomWebSocketServer>();
webSocketServer.Start(app);

app.Run();