using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class SendMessageDTO
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Receiver ID must be a positive integer.")]
    public int receiverId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Message cannot be empty.")]
    public string message { get; set; } = null!;
}
