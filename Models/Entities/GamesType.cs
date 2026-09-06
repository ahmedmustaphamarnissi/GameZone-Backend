using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class GamesType
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
}
