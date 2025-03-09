using WebSocketBoilerplate;

namespace DataAccess.ModelDtos;

public class GameDto : BaseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<PlayerDto> Players { get; set; } = new();
    public List<QuestionDto> Questions { get; set; } = new();
    public Guid CreatedBy { get; set; }

    //Constructor for mapping from Game entity
    public GameDto(DataAccess.Models.Game game)
    {
        Id = game.Id;
        Name = game.Name;
        CreatedBy = game.CreatedBy;

        //Map Players
        Players = game.Players?.Select(p => new PlayerDto(p)).ToList() ?? new();

        //Map Questions
        Questions = game.Questions?.Select(q => new QuestionDto(q)).ToList() ?? new();
        
    }
}