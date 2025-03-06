using WebSocketBoilerplate;
using System;

namespace Api.EventHandlers.Dtos;

public class PlayerAnswerDto : BaseDto
{
    public string PlayerId { get; set; }
    public string QuestionId { get; set; }
    public string? SelectedOptionId { get; set; }
    public DateTime? AnswerTimestamp { get; set; }
}