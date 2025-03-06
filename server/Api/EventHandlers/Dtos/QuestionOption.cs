using WebSocketBoilerplate;

namespace Api.EventHandlers.Dtos;

public class QuestionOptionDto : BaseDto
{
    public string Id { get; set; }
    public string? QuestionId { get; set; }
    public string OptionText { get; set; }
    public bool IsCorrect { get; set; }
}