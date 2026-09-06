using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class NavbarStateDTO
{
    public string firstName { get; set; } = null!;
    public string lastName { get; set; } = null!;
    public string? userPicture { get; set; }

    public int notificationCount { get; set; }
    public int friendRequestsCount { get; set; }
    public int wishlistCount { get; set; }
    public int newMessagesCount { get; set; }
}
