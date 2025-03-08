using Api.EventHandlers.EventMessageDtos;
using Api.WebSockets;
using DataAccess.Models;
using Fleck;
using WebSocketBoilerplate;
using Api.Services;
using DataAccess.ModelDtos;
using Microsoft.EntityFrameworkCore;

namespace Api.EventHandlers
{
    public class AdminStartsGameEventHandler : BaseEventHandler<AdminStartsGameDto>
    {
        private readonly IConnectionManager _connectionManager;
        private readonly KahootDbContext _dbContext;
        private readonly ILogger<AdminStartsGameEventHandler> _logger;
        private readonly QuestionBroadcastService _questionBroadcastService;

        public AdminStartsGameEventHandler(
            IConnectionManager connectionManager, 
            KahootDbContext dbContext, 
            ILogger<AdminStartsGameEventHandler> logger,
            QuestionBroadcastService questionBroadcastService)
        {
            _connectionManager = connectionManager;
            _dbContext = dbContext;
            _logger = logger;
            _questionBroadcastService = questionBroadcastService;
        }

        public override async Task Handle(AdminStartsGameDto dto, IWebSocketConnection socket)
        {
            if (dto == null || string.IsNullOrEmpty(dto.GameId))
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid game selection. A valid GameId is required.",
                    requestId = dto?.requestId
                });
                return;
            }

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

            //Fetch the game from the database (Pre-existing)
            var gameEntity = await _dbContext.Games
                .Include(g => g.Questions)
                .ThenInclude(q => q.QuestionOptions)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (gameEntity == null)
            {
                _logger.LogWarning("Game with ID {GameId} not found.", gameId);
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Game not found.",
                    requestId = dto.requestId
                });
                return;
            }

            _logger.LogDebug("Game {GameName} retrieved, broadcasting start...", gameEntity.Name);

            //Convert GameEntity to DTO
            var gameDto = new GameDto(gameEntity); //Let the constructor handle mapping

            //Broadcast game start
            await _connectionManager.BroadcastToTopic("lobby", gameDto);
            _logger.LogDebug("Game {GameName} started.", gameEntity.Name);

            //Broadcast questions for this game
            await _questionBroadcastService.BroadcastQuestionsForGameAsync(gameEntity.Id, gameEntity.Id.ToString());
        }
    }
}
