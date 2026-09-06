using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class GetBlockedFriendsDto
{
    public List<BlockedFriendsDto>? blockedFriends { get; set; }
    public int blockedFriendsCount { get; set; }
}
