using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class CommentDTO
{
    public int commentId { get; set; }
    public int gameId { get; set; }
    public string userName { get; set; } = null!;
    public string? userImage { get; set; }
    public int userId { get; set; }
    public DateTime commentDate { get; set; }
    public string commentText { get; set; } = null!;
}
