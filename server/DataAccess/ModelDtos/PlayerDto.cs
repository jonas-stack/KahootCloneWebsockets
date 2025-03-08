namespace DataAccess.ModelDtos;

public class PlayerDto
{
    public Guid Id { get; set; } 
    public Guid? GameId { get; set; }
    public string Nickname { get; set; }

    public List<PlayerAnswerDto> PlayerAnswers { get; set; } = new();
    public List<RoundResultDto> RoundResults { get; set; } = new();

    //Constructor for entity mapping
    public PlayerDto(DataAccess.Models.Player player)
    {
        Id = player.Id;
        Nickname = player.Nickname;
        GameId = player.GameId;

        PlayerAnswers = player.PlayerAnswers?.Select(a => new PlayerAnswerDto(a)).ToList() ?? new();
        RoundResults = player.RoundResults?.Select(r => new RoundResultDto(r)).ToList() ?? new();
    }

    //Parameterless constructor for serialization (if needed)
    public PlayerDto() { }
}