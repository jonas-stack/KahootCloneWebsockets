using Api.EventHandlers.EventMessageDtos;
using Api.WebSockets;
using DataAccess.Models;
using Fleck;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using DataAccess.ModelDtos;
using WebSocketBoilerplate;

namespace Api.EventHandlers
{
    public class BroadcastRoundResultsEventHandler : BaseEventHandler<RoundResultDto>
    {
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<BroadcastRoundResultsEventHandler> _logger;

        public BroadcastRoundResultsEventHandler(IConnectionManager connectionManager, ILogger<BroadcastRoundResultsEventHandler> logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }

        public override async Task Handle(RoundResultDto dto, IWebSocketConnection socket)
        {
            if (dto == null || string.IsNullOrEmpty(dto.GameId))
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid round results data.",
                    requestId = dto?.requestId
                });
                return;
            }

            string topic = dto.GameId; // using game id (as string) as topic
            _logger.LogDebug("Broadcasting round results for game {GameId}", dto.GameId);
            await _connectionManager.BroadcastToTopic(topic, dto);
        }
    }
}