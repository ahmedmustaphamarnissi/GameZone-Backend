using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class MessageDTO
{
    public int messageId { get; set; }
    public string message { get; set; } = null!;
    public DateTime sentAt { get; set; }
    public DateTime? readAt { get; set; }
    public bool isMine { get; set; }
    public bool isRemovedBySender { get; set; }
    public bool isRemovedByReceiver { get; set; }
}
