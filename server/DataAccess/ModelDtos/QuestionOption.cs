namespace DataAccess.ModelDtos;

public class QuestionOptionDto
{
    public Guid Id { get; set; }  // ✅ Removed "required" and initialized in constructor
    public Guid? QuestionId { get; set; }  // ✅ Same fix as above
    public string OptionText { get; set; }  // ✅ Ensured string is initialized
    public bool IsCorrect { get; set; } 

    // ✅ Constructor for mapping from entity
    public QuestionOptionDto(DataAccess.Models.QuestionOption option)
    {
        Id = option.Id;
        QuestionId = option.QuestionId;
        OptionText = option.OptionText;
        IsCorrect = option.IsCorrect;
    }

    // ✅ Parameterless constructor for serialization
    public QuestionOptionDto() { }
}