using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class RequirementType
{
    public int RequirementTypeId { get; set; }

    public string RequirementTypeName { get; set; } = null!;

    public virtual ICollection<SystemRequirement> SystemRequirements { get; set; } = new List<SystemRequirement>();
}
