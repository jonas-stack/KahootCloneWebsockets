using Api.WebSockets;
using Fleck;
using WebSocketBoilerplate;
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
            if (dto == null || dto.PlayerId == Guid.Empty || dto.GameId == Guid.Empty)
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid game selection. A valid GameId is required.",
                    requestId = dto?.requestId
                });
                return;
            }

            string topic = dto.GameId.ToString(); // GameId now acts as the topic

            _logger.LogDebug("Player '{PlayerId}' ({Nickname}) is attempting to join game '{GameId}'.", dto.PlayerId, dto.Nickname, topic);

            // Add the player to the game topic
            await _connectionManager.AddToTopic(topic, dto.PlayerId.ToString());
            _logger.LogDebug("Player '{PlayerId}' added to game '{GameId}'.", dto.PlayerId, topic);

            // Retrieve and log the current members in the game
            var members = await _connectionManager.GetMembersFromTopicId(topic);
            _logger.LogDebug("Current players in game '{GameId}': {Members}", topic, string.Join(", ", members));

            // Broadcast to all players in the game that a new member has joined
            var joinMessage = new MemberHasJoinedDto
            {
                MemberId = dto.PlayerId.ToString(),
                Nickname = dto.Nickname,
                requestId = dto.requestId
            };
            await _connectionManager.BroadcastToTopic(topic, joinMessage);
            _logger.LogDebug("Broadcasted join message for player '{PlayerId}' in game '{GameId}'.", dto.PlayerId, topic);

            // Send a confirmation back to the joining client
            socket.SendDto(new ServerConfirmsPlayerJoinDto
            {
                PlayerId = dto.PlayerId,
                Message = $"Successfully joined game {topic}.",
                requestId = dto.requestId
            });
            _logger.LogDebug("Sent join confirmation to player '{PlayerId}'.", dto.PlayerId);
        }
    }
}