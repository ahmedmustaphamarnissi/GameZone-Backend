using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums;

namespace Models.DTO.Filter; 

public class PublicStoreFiltrationRequest
{
    public int[] ? genresIds { get; set; }
    public int[] ? companiesIds { get; set; }
    public int[] ? devicesIds { get; set; }
    public int[]? featuresIds { get; set; }
    public int[]? languagesIds { get; set; }
    public PublicStorePriceFiltration price { get; set; }

}
