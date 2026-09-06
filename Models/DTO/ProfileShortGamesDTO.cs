using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class ProfileShortGamesDTO
{
    public int gameId { get; set; }
    public string gameName { get; set; } = null!;
    public string? gameCover { get; set; }
    public decimal price { get; set; }
    public int discount { get; set; }
    public List<string>? categories { get; set; }
    public DateTime? createdAt { get; set; }
}
