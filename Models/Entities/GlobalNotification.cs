using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class GlobalNotification
{
    public int GlobalNotificationId { get; set; }

    public int NotificationTypeId { get; set; }

    public int CreatedByEmployeeId { get; set; }

    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual Employee CreatedByEmployee { get; set; } = null!;

    public virtual NotificationType NotificationType { get; set; } = null!;
}
