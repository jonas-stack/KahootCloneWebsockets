using WebSocketBoilerplate;
using System.Collections.Generic;

namespace Api.EventHandlers.Dtos;

public class QuestionDto : BaseDto
{
    public required string Id { get; set; }
    public string? GameId { get; set; }
    public required string QuestionText { get; set; }
    public bool Answered { get; set; }
    public List<PlayerAnswerDto> PlayerAnswers { get; set; } = new List<PlayerAnswerDto>();
    public List<QuestionOptionDto> QuestionOptions { get; set; } = new List<QuestionOptionDto>();
}