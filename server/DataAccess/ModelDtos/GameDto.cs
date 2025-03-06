using WebSocketBoilerplate;

namespace Api.EventHandlers.Dtos;

public class GameDto : BaseDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required List<PlayerDto> Players { get; set; }
    public required List<QuestionDto> Questions { get; set; }
    public required List<RoundResultDto> RoundResults { get; set; }
}