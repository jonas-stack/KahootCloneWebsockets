using WebSocketBoilerplate;
using System.Collections.Generic;

namespace Api.EventHandlers.Dtos;

public class PlayerDto : BaseDto
{
    public required string Id { get; set; }
    public string? GameId { get; set; }
    public required string Nickname { get; set; }
    public GameDto? Game { get; set; }
    public List<PlayerAnswerDto> PlayerAnswers { get; set; } = new List<PlayerAnswerDto>();
    public List<RoundResultDto> RoundResults { get; set; } = new List<RoundResultDto>();
}