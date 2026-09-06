using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = null!;

    public int? CountryId { get; set; }

    public string? Website { get; set; }

    public string? LogoPath { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
