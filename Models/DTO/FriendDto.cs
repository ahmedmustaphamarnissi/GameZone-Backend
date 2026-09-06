using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class FriendDto
{
    public int friendRequestId { get; set; }
    public int userId { get; set; }
    public string userName { get; set; } = null!;
    public string? userImage { get; set; }
    public DateTime? friendSince { get; set; }
    public string roleName { get; set; } = null!;
    public string? countryName { get; set; }
    public string? countryCode { get; set; }
}
