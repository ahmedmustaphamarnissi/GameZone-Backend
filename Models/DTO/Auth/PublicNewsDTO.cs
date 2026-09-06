using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO.Auth;

public class PublicNewsDTO
{
        public int GameId { get; set; }
        public string GameName { get; set; } = string.Empty;

       public string? GameLogo { get; set; }
    // Added / Updated
       public string? NewsType { get; set; }

        public DateTime NewsDate { get; set; }
        public string? GameVersion { get; set; }

        public string StatusName { get; set; } = string.Empty;

        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? LogoPath { get; set; }
    
}
