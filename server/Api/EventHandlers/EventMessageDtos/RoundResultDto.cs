using Api.WebSockets;

namespace Api.EventHandlers.EventMessageDtos;

public partial class RoundResultDto : CustomBaseDto
{
    public RoundResultDto()
    {
        eventType = "RoundResult";
    }

    public required Guid GameId { get; set; }
    public int RoundNumber { get; set; }
    public required List<RoundResultEntry> Results { get; set; }
}