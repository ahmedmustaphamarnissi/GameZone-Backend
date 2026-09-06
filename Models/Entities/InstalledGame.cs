using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class InstalledGame
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }

    public long DownloadedBytes { get; set; }

    public bool IsPaused { get; set; }

    public bool IsInstalled { get; set; }

    public string InstallationVersion { get; set; } = null!;

    public DateTime? InstallationDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
