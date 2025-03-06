using WebSocketBoilerplate;

namespace Api.EventHandlers.Dtos;

public class ServerSendsErrorMessageDto : BaseDto
{
    public required string Error { get; set; }
}