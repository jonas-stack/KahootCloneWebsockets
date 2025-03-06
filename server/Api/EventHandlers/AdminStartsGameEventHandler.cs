using Api.EventHandlers.Dtos;
using Api.WebSockets;
using Fleck;
using WebSocketBoilerplate;
using DataAccess.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Api.EventHandlers.Utility;

namespace Api.EventHandlers
{
    // Inherit from BaseEventHandler<GameDto> to automatically get the event routing functionality.
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
            if (dto == null || string.IsNullOrEmpty(dto.Id))
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid game data.",
                    requestId = dto?.requestId
                });
                return;
            }

            _logger.LogDebug("AdminStartsGameEventHandler invoked for game: {GameId}", dto.Id);

            // Map the GameDto to an entity.
            var gameEntity = dto.ToEntity();

            // Save the game entity to the database.
            _dbContext.Games.Add(gameEntity);
            await _dbContext.SaveChangesAsync();
            

            // Optionally, add the current client (admin) to the "lobby" topic.
            var clientId = await _connectionManager.GetClientIdFromSocketId(socket.ConnectionInfo.Id.ToString());
            await _connectionManager.AddToTopic("lobby", clientId);

            
            // Broadcast the game start event to all clients in the lobby.
            await _connectionManager.BroadcastToTopic("lobby", dto);
        }
    }
}
