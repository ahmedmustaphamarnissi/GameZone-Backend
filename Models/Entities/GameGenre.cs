using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class GameGenre
{
    public int Id { get; set; }

    public int GameId { get; set; }

    public int GenreId { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual GamesType Genre { get; set; } = null!;
}
