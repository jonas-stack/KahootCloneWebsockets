using Api.EventHandlers.Dtos;
using Api.EventHandlers.Utility;
using Api.WebSockets;
using Fleck;
using WebSocketBoilerplate;
using DataAccess.Models;

namespace Api.EventHandlers;

public class AdminStartsGameEventHandler : BaseEventHandler<GameDto>
{
    private readonly IConnectionManager _connectionManager;
    private readonly KahootDbContext _dbContext;

    public AdminStartsGameEventHandler(IConnectionManager connectionManager, KahootDbContext dbContext)
    {
        _connectionManager = connectionManager;
        _dbContext = dbContext;
    }

    public override async Task Handle(GameDto dto, IWebSocketConnection socket)
    {
        // Validate the game DTO
        if (dto == null || string.IsNullOrEmpty(dto.Id))
        {
            await socket.Send("Invalid game data.");
            return;
        }

        // Map DTO to entity
        var gameEntity = dto.ToEntity();

        // Save to database
        _dbContext.Games.Add(gameEntity);
        await _dbContext.SaveChangesAsync();

        // Broadcast the game start event to all clients in the "lobby" topic
        await _connectionManager.BroadcastToTopic("lobby", dto);

        // Optionally, you can log the event or perform additional actions here
    }
}