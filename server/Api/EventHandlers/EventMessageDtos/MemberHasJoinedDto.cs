using Api.WebSockets;

namespace Api.EventHandlers.EventMessageDtos;

public partial class MemberHasJoinedDto : CustomBaseDto
{
    public MemberHasJoinedDto()
    {
        eventType = "MemberHasJoined";
    }

    public required string MemberId { get; set; } // The ID of the player who joined
    public required string Nickname { get; set; } // The nickname of the player
}