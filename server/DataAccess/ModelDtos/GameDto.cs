using WebSocketBoilerplate;

namespace DataAccess.ModelDtos;

public class GameDto : BaseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<PlayerDto> Players { get; set; }
    public List<QuestionDto> Questions { get; set; }
    public List<RoundResultDto> RoundResults { get; set; }
    public Guid CreatedBy { get; set; }
}