using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class SystemRequirement
{
    public int SystemRequirementId { get; set; }

    public int GameId { get; set; }

    public int DeviceId { get; set; }

    public int RequirementTypeId { get; set; }

    public string? OperatingSystem { get; set; }

    public string? Processor { get; set; }

    public string? Memory { get; set; }

    public string? Graphics { get; set; }

    public string? DirectX { get; set; }

    public string? Storage { get; set; }

    public string? SoundCard { get; set; }

    public string? Network { get; set; }

    public string? AdditionalNotes { get; set; }

    public virtual Device Device { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;

    public virtual RequirementType RequirementType { get; set; } = null!;
}
