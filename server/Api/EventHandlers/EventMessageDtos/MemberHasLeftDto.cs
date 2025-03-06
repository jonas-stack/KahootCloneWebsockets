using WebSocketBoilerplate;

namespace Api.EventHandlers.Dtos;

public class MemberHasLeftDto : BaseDto
{
    public string? MemberId { get; set; }
    
}