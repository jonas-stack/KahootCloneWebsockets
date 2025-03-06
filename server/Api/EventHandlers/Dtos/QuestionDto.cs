using WebSocketBoilerplate;
using System.Collections.Generic;

namespace Api.EventHandlers.Dtos;

public class QuestionDto : BaseDto
{
    public string Id { get; set; }
    public string? GameId { get; set; }
    public string QuestionText { get; set; }
    public bool Answered { get; set; }
    public List<PlayerAnswerDto> PlayerAnswers { get; set; } = new List<PlayerAnswerDto>();
    public List<QuestionOptionDto> QuestionOptions { get; set; } = new List<QuestionOptionDto>();
}