using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class LanguageDTO
{
     public int languageId { get; set; }
     public string languageName { get; set; } = null!;
     public string? languageCode { get; set; }


}
