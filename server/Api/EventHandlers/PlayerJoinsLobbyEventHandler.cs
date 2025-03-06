using Api.EventHandlers.Dtos;
using Api.WebSockets;
using Fleck;
using WebSocketBoilerplate;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Api.EventHandlers
{
    public class PlayerJoinsLobbyEventHandler : BaseEventHandler<PlayerJoinsLobbyDto>
    {
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<PlayerJoinsLobbyEventHandler> _logger;

        public PlayerJoinsLobbyEventHandler(IConnectionManager connectionManager, ILogger<PlayerJoinsLobbyEventHandler> logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }

        public override async Task Handle(PlayerJoinsLobbyDto dto, IWebSocketConnection socket)
        {
            // Validate the incoming DTO.
            if (dto == null || string.IsNullOrEmpty(dto.PlayerId))
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid join lobby data.",
                    requestId = dto?.requestId // Provides existing requestId or null if not available.
                });
                return;
            }

            _logger.LogDebug($"Player '{dto.PlayerId}' with nickname '{dto.Nickname}' is attempting to join the lobby.");

            // Add the player to the "lobby" topic.
            await _connectionManager.AddToTopic("lobby", dto.PlayerId);
            _logger.LogDebug($"Player '{dto.PlayerId}' added to 'lobby' topic.");

            // Broadcast to all clients in the lobby that a new member has joined.
            await _connectionManager.BroadcastToTopic("lobby", new MemberHasJoinedDto
            {
                MemberId = dto.PlayerId,
                Nickname = dto.Nickname,
                requestId = dto.requestId
            });
            _logger.LogDebug($"Broadcasted join message for player '{dto.PlayerId}'.");

            // Send a confirmation back to the joining client.
            socket.SendDto(new ServerConfirmsPlayerJoinDto
            {
                PlayerId = dto.PlayerId,
                Message = "Successfully joined the lobby.",
                requestId = dto.requestId
            });
            _logger.LogDebug($"Sent join confirmation to player '{dto.PlayerId}'.");
        }
    }
}
