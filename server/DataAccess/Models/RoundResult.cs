using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class RoundResult
{
    public string Id { get; set; } = null!;

    public string? GameId { get; set; }

    public int RoundNumber { get; set; }

    public string? PlayerId { get; set; }

    public int Score { get; set; }

    public virtual Game? Game { get; set; }

    public virtual Player? Player { get; set; }
}
