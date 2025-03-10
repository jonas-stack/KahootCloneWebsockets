using Api.WebSockets;

namespace Api.EventHandlers.EventMessageDtos;

public partial class AnswerValidationDto : CustomBaseDto
{
    public AnswerValidationDto()
    {
        eventType = "AnswerValidation";
    }

    public required Guid PlayerId { get; set; }
    public required Guid QuestionId { get; set; }
    public bool IsCorrect { get; set; }
    public int ScoreAwarded { get; set; }
}