namespace DataAccess.ModelDtos;

public class RoundResultDto
{
    public Guid Id { get; set; } // ✅ Removed "required" to allow proper initialization
    public Guid? GameId { get; set; }
    public int RoundNumber { get; set; }
    public Guid? PlayerId { get; set; }
    public int Score { get; set; }

    // ✅ Constructor for mapping from entity
    public RoundResultDto(DataAccess.Models.RoundResult result)
    {
        Id = result.Id;
        GameId = result.GameId;
        PlayerId = result.PlayerId;
        RoundNumber = result.RoundNumber;
        Score = result.Score;
    }

    // ✅ Parameterless constructor for serialization
    public RoundResultDto() { }
}