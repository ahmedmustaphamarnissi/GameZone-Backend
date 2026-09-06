using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class GamesFeature
{
    public int Id { get; set; }

    public int FeatureId { get; set; }

    public int GameId { get; set; }

    public virtual Feature Feature { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;
}
