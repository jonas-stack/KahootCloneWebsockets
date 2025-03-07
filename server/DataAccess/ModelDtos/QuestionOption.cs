using WebSocketBoilerplate;

namespace DataAccess.ModelDtos;

public class QuestionOptionDto : BaseDto
{
    public required string Id { get; set; }
    public required string QuestionId { get; set; }
    public required string OptionText { get; set; }
    public bool IsCorrect { get; set; }
}