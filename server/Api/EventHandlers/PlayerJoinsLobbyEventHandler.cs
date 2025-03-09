using Api.WebSockets;
using Fleck;
using WebSocketBoilerplate;
using Api.EventHandlers.EventMessageDtos;
using Service;

namespace Api.EventHandlers
{
    public class PlayerJoinsLobbyEventHandler : BaseEventHandler<PlayerJoinsLobbyDto>
    {
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<PlayerJoinsLobbyEventHandler> _logger;
        private readonly PlayerManagementService _playerManagementService; // Use new service

        public PlayerJoinsLobbyEventHandler(
            IConnectionManager connectionManager,
            ILogger<PlayerJoinsLobbyEventHandler> logger,
            PlayerManagementService playerManagementService)
        {
            _connectionManager = connectionManager;
            _logger = logger;
            _playerManagementService = playerManagementService;
        }

        public override async Task Handle(PlayerJoinsLobbyDto dto, IWebSocketConnection socket)
        {
            if (dto == null || string.IsNullOrEmpty(dto.PlayerId) || string.IsNullOrEmpty(dto.GameId))
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid game selection. A valid GameId is required.",
                    requestId = dto?.requestId
                });
                return;
            }

            string topic = dto.Topic; // GameId is the topic

            _logger.LogDebug("Player '{PlayerId}' ({Nickname}) is joining game '{GameId}'.", dto.PlayerId, dto.Nickname, topic);

            // Add player to the database
            await _playerManagementService.AddPlayerAsync(dto.PlayerId, dto.Nickname, dto.GameId);

            // Add player to the WebSocket topic
            await _connectionManager.AddToTopic(topic, dto.PlayerId);

            // Broadcast to all players in the topic
            await _connectionManager.BroadcastToTopic(topic, dto);
        }
    }
}