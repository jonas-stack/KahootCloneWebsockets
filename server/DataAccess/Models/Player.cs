using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Player
{
    public Guid Id { get; set; }

    public Guid? GameId { get; set; }

    public string Nickname { get; set; } = null!;

    public virtual Game? Game { get; set; }

    public virtual ICollection<PlayerAnswer> PlayerAnswers { get; set; } = new List<PlayerAnswer>();

    public virtual ICollection<RoundResult> RoundResults { get; set; } = new List<RoundResult>();
}
