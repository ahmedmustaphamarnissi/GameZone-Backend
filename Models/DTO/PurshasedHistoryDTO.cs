using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class PurshasedHistoryDTO
{
    public List<PurshasedHistoryItem>? PurshasedHistoryItems { get; set; } = new List<PurshasedHistoryItem>();
    public int TotalCount { get; set; }
    public decimal TotalCost { get; set; }
}
