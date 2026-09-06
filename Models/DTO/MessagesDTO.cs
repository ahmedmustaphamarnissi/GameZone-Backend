using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class MessagesDTO
{
    public int conversationsCount { get; set; }
    public List<ConversationItemDTO> conversations { get; set; } = [];
}
