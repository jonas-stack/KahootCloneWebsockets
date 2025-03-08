namespace DataAccess.ModelDtos;

public class QuestionDto
{
    public Guid Id { get; set; }
    public Guid? GameId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public bool Answered { get; set; }

    public List<PlayerAnswerDto> PlayerAnswers { get; set; } = new();
    public List<QuestionOptionDto> QuestionOptions { get; set; } = new();

    // Constructor for mapping from entity
    public QuestionDto(DataAccess.Models.Question question)
    {
        Id = question.Id;
        GameId = question.GameId;
        QuestionText = question.QuestionText ?? string.Empty; // Ensuring no null values
        Answered = question.Answered;

        PlayerAnswers = question.PlayerAnswers?.Select(a => new PlayerAnswerDto(a)).ToList() ?? new();
        QuestionOptions = question.QuestionOptions?.Select(o => new QuestionOptionDto(o)).ToList() ?? new();
    }

    //Parameterless constructor for serialization
    public QuestionDto() { }
}