using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class CompaniesDTO
{
    public int companyId { get; set;}
    public string companyName { get; set; } = null!;
    public string countryName { get; set;} = null!;
    public string? companyCover { get; set; }
    public string countryCode { get; set;} = null!;
    public int gamesCount { get; set; }
}
