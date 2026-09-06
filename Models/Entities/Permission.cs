using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class Permission
{
    public int PermissionId { get; set; }

    public string PermissionName { get; set; } = null!;

    public virtual ICollection<GradePermission> GradePermissions { get; set; } = new List<GradePermission>();
}
