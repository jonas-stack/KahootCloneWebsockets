using Api.WebSockets;

namespace Api.EventHandlers.EventMessageDtos;

public partial class RoundResultEntry : CustomBaseDto
{
    public required Guid PlayerId { get; set; }
    public int Score { get; set; }
}