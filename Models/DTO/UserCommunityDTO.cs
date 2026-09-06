using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTO.Auth;

namespace Models.DTO;

public class UserCommunityDTO
{
    public List<SearchOnFriendsDTO> users { get; set; } = new();
    public List<PublicNewsDTO> news { get; set; } = new();
    public List<FriendsActivitiesItem> friendsActivities { get; set; } = new();
}
