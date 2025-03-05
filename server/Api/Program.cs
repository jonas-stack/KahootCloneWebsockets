using Api.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// Tilføj nødvendige services
builder.Services.AddSingleton<IConnectionManager, DictionaryConnectionManager>();
builder.Services.AddSingleton<CustomWebSocketServer>();
builder.Services.AddLogging();

var app = builder.Build();

// Konfigurer middleware og endpoints
app.UseWebSockets();

var webSocketServer = app.Services.GetRequiredService<CustomWebSocketServer>();
webSocketServer.Start(app);

app.Run();
