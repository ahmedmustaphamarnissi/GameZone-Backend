using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums;

namespace Models.DTO;

public class PerformActionOnFriendRequestDTO
{
    public int friendRequestId { get; set; }
    public FriendRequestActions action { get; set;}
    public DateTime respondedDate { get; set; }
}
