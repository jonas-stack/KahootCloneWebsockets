using WebSocketBoilerplate;

namespace Api.EventHandlers.EventMessageDtos;

public class MemberHasLeftDto : BaseDto
{
    public string? MemberId { get; set; }
    
}