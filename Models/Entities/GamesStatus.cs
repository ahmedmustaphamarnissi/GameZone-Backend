using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class GamesStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? StatusDescription { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
