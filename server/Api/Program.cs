using System.Reflection;
using Api.WebSockets;
using Api.EventHandlers;
using Api.Services;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using WebSocketBoilerplate;

var builder = WebApplication.CreateBuilder(args);

// Register services.
builder.Services.AddSingleton<IConnectionManager, DictionaryConnectionManager>();
builder.Services.AddSingleton<CustomWebSocketServer>();
builder.Services.AddSingleton<IEventHandlersService, EventHandlersService>();
builder.Services.AddScoped<QuestionBroadcastService>();

// Scan the currently executing assembly (typically your API project) for event handlers and DTO types.
builder.Services.InjectEventHandlers(Assembly.GetExecutingAssembly());

// Load and scan the DataAccess assembly (where your DTOs like GameDto are defined) for event handlers and DTO types.
builder.Services.InjectEventHandlers(Assembly.Load("DataAccess"));

// Register DbContext.
builder.Services.AddDbContext<KahootDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddLogging();

var app = builder.Build();

app.UseWebSockets();
var webSocketServer = app.Services.GetRequiredService<CustomWebSocketServer>();
webSocketServer.Start(app);

app.Run();