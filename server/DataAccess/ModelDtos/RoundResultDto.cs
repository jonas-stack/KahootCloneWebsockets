using WebSocketBoilerplate;

namespace DataAccess.ModelDtos;

public class RoundResultDto : BaseDto
{
    public required string Id { get; set; }
    public string? GameId { get; set; }
    public int RoundNumber { get; set; }
    public string? PlayerId { get; set; }
    public int Score { get; set; }
    public GameDto? Game { get; set; }
    public PlayerDto? Player { get; set; }
}