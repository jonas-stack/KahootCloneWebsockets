using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class PlayerAnswer
{
    public Guid PlayerId { get; set; }

    public Guid QuestionId { get; set; }

    public Guid? SelectedOptionId { get; set; }

    public DateTime? AnswerTimestamp { get; set; }

    public virtual Player Player { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;

    public virtual QuestionOption? SelectedOption { get; set; }
}
