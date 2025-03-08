namespace DataAccess.ModelDtos;

public class PlayerAnswerDto 
{
    public Guid PlayerId { get; set; }
    public Guid QuestionId { get; set; } 
    public Guid? SelectedOptionId { get; set; }
    public DateTime? AnswerTimestamp { get; set; }

    // ✅ Constructor for mapping from entity
    public PlayerAnswerDto(DataAccess.Models.PlayerAnswer answer)
    {
        PlayerId = answer.PlayerId;
        QuestionId = answer.QuestionId;
        SelectedOptionId = answer.SelectedOptionId;
        AnswerTimestamp = answer.AnswerTimestamp;
    }

    // ✅ Parameterless constructor for serialization
    public PlayerAnswerDto() { }
}