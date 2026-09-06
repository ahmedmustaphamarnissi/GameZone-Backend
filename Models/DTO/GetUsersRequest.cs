using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums;
using Models.Data.enums.Sorts;

namespace Models.DTO;

public class GetUsersRequest
{
    public int pageNumber { get; set; } = 1;
    public int pageSize { get; set; } = 8;
    public UsersOrderBy orderBy { get; set; } = UsersOrderBy.FriendRequestsFirst;
    public string searchText { get; set; } = string.Empty;
}
