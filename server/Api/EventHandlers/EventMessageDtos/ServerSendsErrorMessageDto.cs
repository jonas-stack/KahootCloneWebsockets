using WebSocketBoilerplate;

namespace Api.EventHandlers.EventMessageDtos;

public class ServerSendsErrorMessageDto : BaseDto
{
    public required string Error { get; set; }
}