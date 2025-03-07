using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Question
{
    public Guid Id { get; set; }

    public Guid? GameId { get; set; }

    public string QuestionText { get; set; } = null!;

    public bool Answered { get; set; }

    public virtual Game? Game { get; set; }

    public virtual ICollection<PlayerAnswer> PlayerAnswers { get; set; } = new List<PlayerAnswer>();

    public virtual ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();
}
