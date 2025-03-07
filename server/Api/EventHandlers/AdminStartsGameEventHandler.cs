using Api.EventHandlers.EventMessageDtos;
using Api.WebSockets;
using DataAccess.Models;
using DataAccess.ModelDtos.Utility;
using Fleck;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebSocketBoilerplate;
using Api.Services;
using DataAccess.ModelDtos;

namespace Api.EventHandlers
{
    public class AdminStartsGameEventHandler : BaseEventHandler<GameDto>
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

        public override async Task Handle(GameDto dto, IWebSocketConnection socket)
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
            dto.Id = Guid.NewGuid();
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
            dto.CreatedBy = clientGuid;
            _logger.LogDebug("Game created by client: {ClientGuid}", dto.CreatedBy);

            // Map DTO to entity and save.
            var gameEntity = dto.ToEntity();
            _dbContext.Games.Add(gameEntity);
            await _dbContext.SaveChangesAsync();
            _logger.LogDebug("Game saved with Id: {GameId}", dto.Id);

            // Add admin to lobby and broadcast game start.
            await _connectionManager.AddToTopic("lobby", clientIdString);
            await _connectionManager.BroadcastToTopic("lobby", dto);
            _logger.LogDebug("Broadcasted game start for game: {GameId}", dto.Id);

            // Broadcast questions associated with this game (using game id as topic).
            await _questionBroadcastService.BroadcastQuestionsForGameAsync(dto.Id, dto.Id.ToString());
        }
    }
}
