using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int PersonId { get; set; }

    public string Password { get; set; } = null!;

    public int PictureId { get; set; }

    public decimal Salary { get; set; }

    public int StatusId { get; set; }

    public virtual ICollection<GlobalNotification> GlobalNotifications { get; set; } = new List<GlobalNotification>();

    public virtual Person Person { get; set; } = null!;

    public virtual EmployeesPicture Picture { get; set; } = null!;

    public virtual PeopleStatus Status { get; set; } = null!;
}
