using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class WishListGamesDTO
{
    public List<generalGameDTO> games { get; set; }  = new List<generalGameDTO>();
    public int countItems { get; set; }
}
