using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Game
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid CreatedBy { get; set; }

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<RoundResult> RoundResults { get; set; } = new List<RoundResult>();
}
