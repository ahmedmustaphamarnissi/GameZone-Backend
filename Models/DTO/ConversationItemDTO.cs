using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class ConversationItemDTO
{
    public int userId { get; set; }
    public string userName { get; set; } = null!;
    public string? picture { get; set; }
    public int nonReadMessagesCount { get; set; }
}
