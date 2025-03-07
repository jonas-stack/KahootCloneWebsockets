using Api.WebSockets;
using Fleck;
using WebSocketBoilerplate;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Api.EventHandlers.EventMessageDtos;

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
                    requestId = dto?.requestId
                });
                return;
            }

            _logger.LogDebug("Player '{PlayerId}' with nickname '{Nickname}' is attempting to join topic '{Topic}'.", dto.PlayerId, dto.Nickname, dto.Topic);

            // Add the player to the specified topic.
            await _connectionManager.AddToTopic(dto.Topic, dto.PlayerId);
            _logger.LogDebug("Player '{PlayerId}' added to topic '{Topic}'.", dto.PlayerId, dto.Topic);

            // Retrieve the current list of members in that topic.
            var members = await _connectionManager.GetMembersFromTopicId(dto.Topic);
            _logger.LogDebug("Current members in '{Topic}': {Members}", dto.Topic, string.Join(", ", members));

            // Broadcast to all clients in the specified topic that a new member has joined.
            var joinMessage = new MemberHasJoinedDto
            {
                MemberId = dto.PlayerId,
                Nickname = dto.Nickname,
                requestId = dto.requestId
            };
            await _connectionManager.BroadcastToTopic(dto.Topic, joinMessage);
            _logger.LogDebug("Broadcasted join message for player '{PlayerId}' in topic '{Topic}'.", dto.PlayerId, dto.Topic);

            // Send a confirmation back to the joining client.
            socket.SendDto(new ServerConfirmsPlayerJoinDto
            {
                PlayerId = dto.PlayerId,
                Message = "Successfully joined the topic.",
                requestId = dto.requestId
            });
            _logger.LogDebug("Sent join confirmation to player '{PlayerId}'.", dto.PlayerId);
        }
    }
}
