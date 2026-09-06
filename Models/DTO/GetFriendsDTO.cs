using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class GetFriendsDTO
{
    public List<FriendDto>? friends { get; set;}
    public int friendsCount { get; set; }
}
