using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class PlayerAnswer
{
    public string PlayerId { get; set; } = null!;

    public string QuestionId { get; set; } = null!;

    public string? SelectedOptionId { get; set; }

    public DateTime? AnswerTimestamp { get; set; }

    public virtual Player Player { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;

    public virtual QuestionOption? SelectedOption { get; set; }
}
