using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class RoundResult
{
    public Guid Id { get; set; }

    public Guid? GameId { get; set; }

    public int RoundNumber { get; set; }

    public Guid? PlayerId { get; set; }

    public int Score { get; set; }

    public virtual Game? Game { get; set; }

    public virtual Player? Player { get; set; }
}
