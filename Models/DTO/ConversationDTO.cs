using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class ConversationDTO
{
    public int messagesCount { get; set; }
    public List<MessageDTO> messages { get; set; } = [];
}
