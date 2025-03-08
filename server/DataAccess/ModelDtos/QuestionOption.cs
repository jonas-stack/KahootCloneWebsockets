namespace DataAccess.ModelDtos;

public class QuestionOptionDto 
{
    public required string Id { get; set; }
    public required string QuestionId { get; set; }
    public required string OptionText { get; set; }
    public bool IsCorrect { get; set; }
}