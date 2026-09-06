using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class Feature
{
    public int Id { get; set; }

    public string FeatureName { get; set; } = null!;

    public virtual ICollection<GamesFeature> GamesFeatures { get; set; } = new List<GamesFeature>();
}
