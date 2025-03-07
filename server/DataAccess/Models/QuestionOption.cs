﻿using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class QuestionOption
{
    public Guid Id { get; set; }

    public Guid? QuestionId { get; set; }

    public string OptionText { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public virtual ICollection<PlayerAnswer> PlayerAnswers { get; set; } = new List<PlayerAnswer>();

    public virtual Question? Question { get; set; }
}
