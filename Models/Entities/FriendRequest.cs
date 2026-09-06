using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class FriendRequest
{
    public int FriendRequestId { get; set; }

    public int StatusId { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public DateTime SendDate { get; set; }

    public DateTime? RespondedDate { get; set; }

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;

    public virtual FriendRequestStatus Status { get; set; } = null!;
}
