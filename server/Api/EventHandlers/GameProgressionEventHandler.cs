using Api.EventHandlers.EventMessageDtos;
using Api.WebSockets;
using Fleck;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebSocketBoilerplate;

namespace Api.EventHandlers
{
    public class GameProgressionEventHandler : BaseEventHandler<GameProgressionDto>
    {
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<GameProgressionEventHandler> _logger;

        public GameProgressionEventHandler(IConnectionManager connectionManager, ILogger<GameProgressionEventHandler> logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }

        public override async Task Handle(GameProgressionDto dto, IWebSocketConnection socket)
        {
            if (dto == null || dto.GameId == Guid.Empty)
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid game progression data.",
                    requestId = dto?.requestId
                });
                return;
            }

            string topic = dto.GameId.ToString();
            if (dto.CurrentRound < dto.TotalRounds)
            {
                dto.Message = $"Round {dto.CurrentRound} is starting.";
            }
            else
            {
                dto.Message = "Game Over!";
            }
            _logger.LogDebug("Broadcasting game progression for game {GameId}: {Message}", dto.GameId, dto.Message);
            await _connectionManager.BroadcastToTopic(topic, dto);
        }
    }
}