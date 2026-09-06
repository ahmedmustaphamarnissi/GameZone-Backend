using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class EventGame
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public int GameId { get; set; }

    public decimal DiscountPercentage { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;
}
