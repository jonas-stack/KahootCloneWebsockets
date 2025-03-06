using WebSocketBoilerplate;

namespace Api.EventHandlers.Dtos;

public class GameDto : BaseDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<PlayerDto> Players { get; set; }
    public List<QuestionDto> Questions { get; set; }
    public List<RoundResultDto> RoundResults { get; set; }
}