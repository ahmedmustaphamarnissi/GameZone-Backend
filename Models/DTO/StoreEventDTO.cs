using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class StoreEventDTO
{
    public string eventName { get; set; } = null!;
    public decimal maxDiscount { get; set; }
    public List<generalGameDTO>? eventGames { get; set; }
}
