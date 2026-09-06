using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class ReviewDTO
{
    public int reviewId { get; set; }
    public string ?userName { get; set; } = null!;
    public string? userImage { get; set; }
    public int? gameId { get; set; }
    public int userId { get; set; }
    public byte rate { get; set; }
    public string? reviewComment { get; set; }
    public DateTime createdAt { get; set; }
}
