using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO.Filter; 

public class StoreFiltration
{
   public List<CompaniesDTO> ? companies { get; set;}
   public List<CategoriesDTO>? categories { get; set; }
   public List<DevicesDTO>? devices { get; set; }
   public List<FeatureDTO>? features { get; set; }
   public List<LanguageDTO>? languages { get; set; }
}
