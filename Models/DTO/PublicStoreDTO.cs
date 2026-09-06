using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTO.Filter;

namespace Models.DTO;

public class PublicStoreDTO
{
    public List<generalGameDTO> ? topSellers { get; set; }
    public List<generalGameDTO>? newReleases { get; set; }
    public List<generalGameDTO>? commingSoon { get; set; }
    public List<generalGameDTO>? specialOffers { get; set; }
    public List<generalGameDTO>? freeGames { get; set; }

    public StoreFiltration ? filtrations { get; set; } = new StoreFiltration();
}
