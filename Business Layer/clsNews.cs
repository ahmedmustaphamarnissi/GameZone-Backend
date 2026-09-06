using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Data_Access_Layer;
using GameZoneBack.Models;
using Microsoft.Extensions.Configuration;
using Models.DTO;
using Models.DTO.Auth;

namespace Business_Layer;

public class clsNews : BaseService
{

    public clsNews(IConfiguration config ) : base(config) { }
    public async Task<List<PublicNewsDTO>?> GetPublicNews(int page , int items)
    {
        var Data = new NewsData(_config);
        List<PublicNewsDTO>? news = await Data.GetPublicNewsAsync(page, items);
        if (news == null)
            throw new ArgumentException("news are not found");
        return news;
    } 

    public async Task<UserCommunityDTO?> GetUserCommunityAsync(int userId)
    {
        var res = new UserCommunityDTO();
        res.news = await new NewsData(_config).GetPublicNewsAsync(1, 8);
        if (res.news == null)
            throw new ArgumentException("error founding news");
        res.users = await new FriendRequestsData(_config).GetUsersYouMightKhnowAsync(userId);
        res.friendsActivities = await new FriendRequestsData(_config).GetFriendsRecentActivitiesAsync(userId);

        if(res == null)
            throw new ArgumentException("error founding news"); 
        return res;
    }
}
