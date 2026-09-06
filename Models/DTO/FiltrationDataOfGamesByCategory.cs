using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class FiltrationDataOfGamesByCategory
{
    public string ? categoryName { get; set; }
    public decimal maxPrice { get; set; }
    public decimal minPrice { get; set; }
    public List<CompaniesDTO> ? companies { get; set; }
    public List<DevicesDTO>? devices { get; set; }
    public List<LanguageDTO>? languages { get; set; }
    public List<FeatureDTO>? features { get; set; }
}
