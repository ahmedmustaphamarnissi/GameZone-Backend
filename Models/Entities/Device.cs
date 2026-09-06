using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class Device
{
    public int DeviceId { get; set; }

    public string DeviceName { get; set; } = null!;

    public string? IconPath { get; set; }

    public virtual ICollection<GameDevice> GameDevices { get; set; } = new List<GameDevice>();

    public virtual ICollection<SystemRequirement> SystemRequirements { get; set; } = new List<SystemRequirement>();
}
