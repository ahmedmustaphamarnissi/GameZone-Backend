using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class GradePermission
{
    public int Id { get; set; }

    public int PermissionId { get; set; }

    public int GradeId { get; set; }

    public virtual Grade Grade { get; set; } = null!;

    public virtual Permission Permission { get; set; } = null!;
}
