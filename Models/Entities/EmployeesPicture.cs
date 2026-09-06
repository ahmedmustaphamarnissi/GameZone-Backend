using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class EmployeesPicture
{
    public int PictureId { get; set; }

    public string Path { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
