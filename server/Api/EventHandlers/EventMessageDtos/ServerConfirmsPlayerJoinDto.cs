using Api.WebSockets;

namespace Api.EventHandlers.EventMessageDtos;

public partial class ServerConfirmsPlayerJoinDto : CustomBaseDto
{
    public ServerConfirmsPlayerJoinDto()
    {
        eventType = "ServerConfirmsPlayerJoin";
    }

    public required Guid PlayerId { get; set; } // The ID of the player that joined
    public required string Message { get; set; } // Confirmation message
}