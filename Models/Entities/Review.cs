using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int GameId { get; set; }

    public int UserId { get; set; }

    public byte Review1 { get; set; }

    public string? ReviewComment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
