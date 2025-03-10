using Api.WebSockets;

namespace Api.EventHandlers.EventMessageDtos;

public partial class MemberHasLeftDto : CustomBaseDto
{
    public MemberHasLeftDto()
    {
        eventType = "MemberHasLeft";
    }

    public required string MemberId { get; set; }
    public string? GameId { get; set; }
}