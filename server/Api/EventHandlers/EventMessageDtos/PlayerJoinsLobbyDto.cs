using WebSocketBoilerplate;
using System.Collections.Generic;

namespace Api.EventHandlers.Dtos
{
    // This DTO is sent by the client when a player wants to join a topic.
    public class PlayerJoinsLobbyDto : BaseDto
    {
        public PlayerJoinsLobbyDto()
        {
            // Set the eventType and default topic.
            eventType = "PlayerJoinsLobby";
            Topic = "lobby"; // default topic is "lobby"
        }

        public required string PlayerId { get; set; }
        public required string Nickname { get; set; }
        // Dynamic topic property so the client can specify a topic (default is "lobby").
        public string Topic { get; set; }
    }

    // This DTO is broadcast to all clients in the topic to announce a new member.
    public class MemberHasJoinedDto : BaseDto
    {
        public MemberHasJoinedDto()
        {
            eventType = "MemberHasJoined";
        }

        public required string MemberId { get; set; }
        public required string Nickname { get; set; }
    }

    // This DTO confirms to the joining client that they have successfully joined the topic.
    public class ServerConfirmsPlayerJoinDto : BaseDto
    {
        public ServerConfirmsPlayerJoinDto()
        {
            eventType = "ServerConfirmsPlayerJoin";
        }

        public required string PlayerId { get; set; }
        public required string Message { get; set; }
    }
}