using Api.WebSockets;

namespace Api.EventHandlers.EventMessageDtos;

public partial class PlayerJoinsLobbyDto : CustomBaseDto
{
    public PlayerJoinsLobbyDto()
    {
        eventType = "PlayerJoinsLobby";
    }

    public required Guid PlayerId { get; set; } // Unique identifier for the player
    public required string Nickname { get; set; } // Player's chosen name
    public required Guid GameId { get; set; } // The game lobby the player is joining
    public string Topic => GameId.ToString(); // GameId is used as the WebSocket topic // GameId is used as the WebSocket topic
}