using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class GameDevice
{
    public int Id { get; set; }

    public int DeviceId { get; set; }

    public int GameId { get; set; }

    public virtual Device Device { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;
}
