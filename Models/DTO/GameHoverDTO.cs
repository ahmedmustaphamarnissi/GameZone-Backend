using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class GameHoverDTO
{
    public int gameId { get; set; }

    public string title { get; set; } = null!;

    public DateTime? lastUpdateDate { get; set; }

    public string shortDescription { get; set; } = null!;

    public decimal rating { get; set; }

    public int reviewsCount { get; set; }

    public List<string> genres { get; set; } = [];

    public string? trailer { get; set; }

    public List<string> screenshots { get; set; } = [];
}
