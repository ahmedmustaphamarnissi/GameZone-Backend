using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class ReviewsPutDTO
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "review id must be a positive number.")]
    public int reviewId { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rate must be between 1 and 5.")]
    public byte rate { get; set; }
    public string? reviewComment { get; set; }
}
