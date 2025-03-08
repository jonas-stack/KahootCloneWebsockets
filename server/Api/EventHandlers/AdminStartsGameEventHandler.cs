using Api.EventHandlers.EventMessageDtos;
using Api.WebSockets;
using DataAccess.Models;
using Fleck;
using Microsoft.EntityFrameworkCore;
using WebSocketBoilerplate;

namespace Api.EventHandlers
{
    public class AdminStartsGameEventHandler : BaseEventHandler<AdminStartsGameDto>
    {
        private readonly IConnectionManager _connectionManager;
        private readonly KahootDbContext _dbContext;
        private readonly ILogger<AdminStartsGameEventHandler> _logger;

        public AdminStartsGameEventHandler(IConnectionManager connectionManager, 
            KahootDbContext dbContext, ILogger<AdminStartsGameEventHandler> logger)
        {
            _connectionManager = connectionManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        public override async Task Handle(AdminStartsGameDto dto, IWebSocketConnection socket)
        {
            if (!Guid.TryParse(dto.GameId, out Guid gameId))
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid GameId format.",
                    requestId = dto.requestId
                });
                return;
            }

            _logger.LogDebug("Fetching game with ID: {GameId}", gameId);

            var gameEntity = await _dbContext.Games
                .Include(g => g.Questions)
                .ThenInclude(q => q.QuestionOptions)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (gameEntity == null)
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Game not found.",
                    requestId = dto.requestId
                });
                return;
            }

            _logger.LogDebug("Game {GameName} started, broadcasting to players...", gameEntity.Name);

            await _connectionManager.BroadcastToTopic(dto.GameId, new GameStartedDto { GameId = dto.GameId });

            _logger.LogDebug("Game {GameName} successfully started.", gameEntity.Name);
        }
    }
}
