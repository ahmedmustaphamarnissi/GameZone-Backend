using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class UserNotification
{
    public int UserNotificationId { get; set; }

    public int UserId { get; set; }

    public int NotificationTypeId { get; set; }

    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ReadDate { get; set; }

    public virtual NotificationType NotificationType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
