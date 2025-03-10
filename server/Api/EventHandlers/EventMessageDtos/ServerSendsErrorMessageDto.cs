using Api.WebSockets;

namespace Api.EventHandlers.EventMessageDtos;

public partial class ServerSendsErrorMessageDto : CustomBaseDto
{
    public ServerSendsErrorMessageDto() => eventType = "ServerSendsErrorMessage";
    public required string Error { get; set; }
}