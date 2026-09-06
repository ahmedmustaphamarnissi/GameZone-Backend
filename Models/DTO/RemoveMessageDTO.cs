using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums;

namespace Models.DTO;

public class RemoveMessageDTO
{
    [Required]
    public int messageId { get; set; }

    [Required]
    public MessageRemoveType removeType { get; set; }
}
