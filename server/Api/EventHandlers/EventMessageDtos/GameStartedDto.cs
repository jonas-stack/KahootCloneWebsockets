using Api.WebSockets;

namespace Api.EventHandlers.EventMessageDtos;

public partial class GameStartedDto : CustomBaseDto
{
    public GameStartedDto() => eventType = "GameStarted";
    public Guid GameId { get; set; } 
    public string Message { get; set; } = "The game has started!";
}