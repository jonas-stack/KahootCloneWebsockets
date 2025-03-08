namespace DataAccess.ModelDtos;

public class PlayerDto
{
    public required string Id { get; set; }
    public string? GameId { get; set; }
    public required string Nickname { get; set; }
    public GameDto? Game { get; set; }
    public List<PlayerAnswerDto> PlayerAnswers { get; set; } = new List<PlayerAnswerDto>();
    public List<RoundResultDto> RoundResults { get; set; } = new List<RoundResultDto>();
}