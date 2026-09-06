using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string EventName { get; set; } = null!;

    public string? EventDescription { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? BannerImagePath { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdated { get; set; }

    public virtual ICollection<EventGame> EventGames { get; set; } = new List<EventGame>();
}
