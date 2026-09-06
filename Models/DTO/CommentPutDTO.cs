using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class CommentPutDTO
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "comment id must be a positive number.")]
    public int commentId { get; set; }

    [Required]
    [MinLength(2,ErrorMessage ="Comment text must be at least 2 characters long.")]
    public string CommentText { get; set; } = null!;
}
