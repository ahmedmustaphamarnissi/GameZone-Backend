using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class GameLanguage
{
    public int GameLanguageId { get; set; }

    public int GameId { get; set; }

    public int LanguageId { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual Language Language { get; set; } = null!;
}
