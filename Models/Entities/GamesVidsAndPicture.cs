using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class GamesVidsAndPicture
{
    public int Id { get; set; }

    public int GameId { get; set; }

    /// <summary>
    /// 0 picture 1 video
    /// </summary>
    public bool Type { get; set; }

    public bool IsPrimary { get; set; }

    public string Path { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;
}
