using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int SenderUserId { get; set; }

    public int ReceiverUserId { get; set; }

    public string Message1 { get; set; } = null!;

    public bool IsEncoded { get; set; }

    public DateTime SentAt { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public bool IsDeletedBySender { get; set; }

    public bool IsDeletedByReceiver { get; set; }

    public virtual User ReceiverUser { get; set; } = null!;

    public virtual User SenderUser { get; set; } = null!;
}
