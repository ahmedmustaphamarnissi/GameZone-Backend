using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class GetUsersDTO
{
    public List<SearchOnFriendsDTO>? users { get; set;}
    public int usersCount { get; set;}
}
