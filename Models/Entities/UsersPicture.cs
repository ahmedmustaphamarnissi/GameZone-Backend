using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class UsersPicture
{
    public int Id { get; set; }

    public string Path { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
