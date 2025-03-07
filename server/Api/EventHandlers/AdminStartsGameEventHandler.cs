using Api.EventHandlers.Dtos;
using Api.WebSockets;
using Fleck;
using WebSocketBoilerplate;
using DataAccess.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Api.EventHandlers.Utility;

namespace Api.EventHandlers
{
    public class AdminStartsGameEventHandler : BaseEventHandler<GameDto>
    {
        private readonly IConnectionManager _connectionManager;
        private readonly KahootDbContext _dbContext;
        private readonly ILogger<AdminStartsGameEventHandler> _logger;

        public AdminStartsGameEventHandler(IConnectionManager connectionManager, KahootDbContext dbContext, ILogger<AdminStartsGameEventHandler> logger)
        {
            _connectionManager = connectionManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        public override async Task Handle(GameDto dto, IWebSocketConnection socket)
        {
            // Validate the incoming DTO.
            if (dto == null || string.IsNullOrEmpty(dto.Name))
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid game data.",
                    requestId = dto?.requestId
                });
                return;
            }

            _logger.LogDebug("AdminStartsGameEventHandler invoked for game with name: {GameName}", dto.Name);

            // Always generate a new unique ID for the game.
            dto.Id = Guid.NewGuid();
            _logger.LogDebug("Assigned new game id: {GameId}", dto.Id);

            // Retrieve the proper client id (from the query string)
            // and parse it as a Guid (assuming your connection manager returns a valid Guid string).
            var clientIdString = await _connectionManager.GetClientIdFromSocketId(socket.ConnectionInfo.Id.ToString());
            if (!Guid.TryParse(clientIdString, out Guid clientGuid))
            {
                _logger.LogWarning("Client id {ClientId} is not a valid GUID.", clientIdString);
                socket.SendDto(new ServerSendsErrorMessageDto 
                { 
                    Error = "Invalid client identifier.", 
                    requestId = dto.requestId 
                });
                return;
            }

            // Set the CreatedBy property so the game is associated with its creator.
            dto.CreatedBy = clientGuid;
            _logger.LogDebug("Game created by client: {ClientGuid}", dto.CreatedBy);

            // Map the GameDto to an entity.
            var gameEntity = dto.ToEntity();

            // Save the game entity to the database.
            _dbContext.Games.Add(gameEntity);
            await _dbContext.SaveChangesAsync();
            _logger.LogDebug("Game entity saved to database with Id: {GameId}", dto.Id);

            // Add the current client (admin) to the "lobby" topic.
            await _connectionManager.AddToTopic("lobby", clientIdString);
            _logger.LogDebug("Added client {ClientId} to lobby", clientIdString);

            // Broadcast the game start event to all clients in the lobby.
            await _connectionManager.BroadcastToTopic("lobby", dto);
            _logger.LogDebug("Broadcasted game start event for game: {GameId}", dto.Id);
        }
    }
}
