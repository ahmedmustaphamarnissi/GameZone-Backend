using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums;

namespace Models.DTO
{
    public class SearchOnFriendsDTO
    {
        public int? friendRequestId { get; set; }
        public int userId { get; set; }
        public string userName { get; set; } = null!;
        public string? userImage { get; set; }
        public UsersRelationship relationship { get; set; }
        public int mutualFriendsCount { get; set; }
    }
}
