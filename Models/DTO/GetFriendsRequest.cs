using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums;

namespace Models.DTO;

public class GetFriendsRequest
{
    public int pageNumber { get; set; } = 1;
    public int pageSize { get; set; } = 8;
    public FriendsOrderBy orderBy { get; set; } = FriendsOrderBy.RecentlyAdded;
    public string searchText { get; set; } = string.Empty;

}
