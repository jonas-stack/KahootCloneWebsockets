using Api.WebSockets;

namespace Api.EventHandlers.EventMessageDtos;

public partial class AdminStartsNextRoundDto : CustomBaseDto
{
    public AdminStartsNextRoundDto() => eventType = "AdminStartsNextRound";
    public required Guid GameId { get; set; }
    public int RoundNumber { get; set; }
}