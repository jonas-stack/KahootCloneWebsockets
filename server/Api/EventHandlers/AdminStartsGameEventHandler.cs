using Api.EventHandlers.EventMessageDtos;
using Api.WebSockets;
using DataAccess.Models;
using Fleck;
using WebSocketBoilerplate;
using Api.Services;

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
            if (dto == null || string.IsNullOrEmpty(dto.Name))
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid game data.",
                    requestId = dto?.requestId
                });
                return;
            }

            _logger.LogDebug("Starting game with name: {GameName}", dto.Name);

            // Create new game id and set the creator.
            var gameId = Guid.NewGuid();
            var clientIdString = await _connectionManager.GetClientIdFromSocketId(socket.ConnectionInfo.Id.ToString());
            if (!Guid.TryParse(clientIdString, out Guid clientGuid))
            {
                _logger.LogWarning("Invalid client GUID: {ClientId}", clientIdString);
                socket.SendDto(new ServerSendsErrorMessageDto 
                { 
                    Error = "Invalid client identifier.", 
                    requestId = dto.requestId 
                });
                return;
            }

            // Map DTO to entity and save.
            var gameEntity = new Game()
            {
                Id = gameId,
                Name = dto.Name,
                CreatedBy = clientGuid.ToString()
            };

            _dbContext.Games.Add(gameEntity);
            await _dbContext.SaveChangesAsync();
            _logger.LogDebug("Game saved with Id: {GameId}", gameId);

            // Add admin to lobby and broadcast game start.
            await _connectionManager.AddToTopic("lobby", clientIdString);
            await _connectionManager.BroadcastToTopic("lobby", dto);
            _logger.LogDebug("Broadcasted game start for game: {GameId}", gameId);

            // Broadcast questions associated with this game.
            await _questionBroadcastService.BroadcastQuestionsForGameAsync(gameId, gameId.ToString());
        }
    }
}
