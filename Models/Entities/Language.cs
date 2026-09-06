using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class Language
{
    public int LanguageId { get; set; }

    public string LanguageName { get; set; } = null!;

    public string LanguageCode { get; set; } = null!;

    public virtual ICollection<GameLanguage> GameLanguages { get; set; } = new List<GameLanguage>();
}
