using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class FriendRequestStatus
{
    public int FriendRequestStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<FriendRequest> FriendRequests { get; set; } = new List<FriendRequest>();
}
