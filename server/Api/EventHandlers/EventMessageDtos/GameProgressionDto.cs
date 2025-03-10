using Api.WebSockets;

namespace Api.EventHandlers.EventMessageDtos;

public partial class GameProgressionDto : CustomBaseDto
{
    public GameProgressionDto()
    {
        eventType = "GameProgression";
    }

    public required Guid GameId { get; set; }
    public int CurrentRound { get; set; }
    public int TotalRounds { get; set; }
    public string Message { get; set; } = "";
}