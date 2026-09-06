using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class BlockedFriendsDto
    {
        public int friendRequestId { get; set; }
        public int userId { get; set; }
        public string userName { get; set; } = null!;
        public string? userImage { get; set; }
        public DateTime? blockedSince { get; set; }
    }
}
