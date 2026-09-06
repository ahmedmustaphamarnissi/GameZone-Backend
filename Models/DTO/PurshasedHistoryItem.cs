using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class PurshasedHistoryItem
{
    public int PurshasedId { get; set; }
    public int GameId { get; set; }
    public string GameName { get; set; } = null!;
    public string? GameStatus { get; set; } 
    public string? GameCover { get; set; } 
    public DateTime PurshasedDate { get; set; }
    public decimal PurshasedPrice { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public bool IsDefaultCard { get; set; }
}
