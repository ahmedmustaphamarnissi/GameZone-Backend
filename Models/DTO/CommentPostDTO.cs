using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class CommentPostDTO
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "GameId must be a positive number.")]
    public int gameId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "UserId must be a positive number.")]
    public int userId { get; set; }

    [Required]
    [MinLength(2)]
    public string commentText { get; set; } = null!;
    public DateTime commentDate { get; set; }
}
