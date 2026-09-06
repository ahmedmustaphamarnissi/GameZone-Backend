using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public string GradeName { get; set; } = null!;

    public virtual ICollection<GradePermission> GradePermissions { get; set; } = new List<GradePermission>();

    public virtual ICollection<Person> People { get; set; } = new List<Person>();
}
