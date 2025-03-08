using DataAccess.Models;

namespace DataAccess.ModelDtos
{
    public class GameDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<PlayerDto> Players { get; set; } = new();
        public List<QuestionDto> Questions { get; set; } = new();
        public List<RoundResultDto> RoundResults { get; set; } = new();
        public Guid CreatedBy { get; set; }

        // Constructor to map from Game entity
        public GameDto(Game game)
        {
            Id = game.Id;
            Name = game.Name;
            CreatedBy = Guid.Parse(game.CreatedBy);
        }
    }
}