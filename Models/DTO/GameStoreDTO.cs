using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class GameStoreDTO
{
    public int gameId { get; set; }
    public string gameName { get; set; } = null!;
    public string? gameCover { get; set; }
    public string companyName { get; set; } = null!;
    public string companyCover  { get; set; } = null!;
    public DateTime additionDate { get; set; }
    public string ? gameStatus { get; set; }
    public decimal price { get; set; }
    public int discount { get; set; }
    public List<string> categories { get; set; }

    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int TotalSold { get; set; }
}
