using WebSocketBoilerplate;

namespace DataAccess.ModelDtos;

public class PlayerAnswerDto : BaseDto
{
    public required string PlayerId { get; set; }
    public required string QuestionId { get; set; }
    public string? SelectedOptionId { get; set; }
    public DateTime? AnswerTimestamp { get; set; }
}