using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class NotificationType
{
    public int NotificationTypeId { get; set; }

    public string NotificationTypeName { get; set; } = null!;

    public virtual ICollection<GlobalNotification> GlobalNotifications { get; set; } = new List<GlobalNotification>();

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
