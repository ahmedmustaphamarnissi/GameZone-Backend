using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using GameZoneBack.Models;
using Microsoft.Extensions.Configuration;
using Models.Data.enums;
using Models.DTO;

namespace Business_Layer;

public class clsFriendRequests : BaseService
{
    public clsFriendRequests(IConfiguration config) : base(config) { }

    public async Task<GetFriendRequestDTO?> GetFriendRequestsAsync(
    int userId,
    int pageSize,
    int pageNumber)
    {
        var data = await new Data_Access_Layer.FriendRequestsData(_config).GetFriendRequests(userId, pageSize, pageNumber);
        if (data == null)
            throw new ArgumentException("Error getting friend requests");
        return data;
    }
    public async Task<bool> PerformActionOnFriendRequest(int userId, PerformActionOnFriendRequestDTO dto)
    {
        var result = await new Data_Access_Layer.FriendRequestsData(_config).PerformActionOnFriendRequest(userId, dto);
        if (!result)
            throw new ArgumentException("Error performing action on friend request");

        if (dto.action == Models.Data.enums.FriendRequestActions.Accept && result == true)
        {
            var senderData = await new Data_Access_Layer.FriendRequestsData(_config)
                .GetSenderData(dto.friendRequestId);
            await new Data_Access_Layer.NotificationsData(_config)
                .NotificateUserAsync(senderData.UserId, enNotificationType.FriendRequestAccepted
                , senderData?.UserName ?? "user");

        }

        return result;
    }
    public async Task<GetFriendsDTO?> GetFriendAsync(
    int userId,
    GetFriendsRequest request)

    {
        var result = await new Data_Access_Layer.FriendRequestsData(_config).GetFriendAsync(userId, request);
        if (result == null)
            throw new ArgumentException("Error getting friends");
        return result;
    }

    public async Task<bool> CheckIfExistFriend(int friendRequestId)
    {
        return await new Data_Access_Layer.FriendRequestsData(_config).CheckIfExistFriend(friendRequestId);
    }
    public async Task<bool> DeleteFriendAsync(int friendRequestId)
    {
        var result = await new Data_Access_Layer.FriendRequestsData(_config).DeleteFriendAsync(friendRequestId);
        if (result == null)
            throw new ArgumentException("Error getting friends");
        return result;
    }

    public async Task<GetBlockedFriendsDto?> GetBlockedFriendsAsync(
    int userId,
    GetFriendsRequest request)

    {
        var result = await new Data_Access_Layer.FriendRequestsData(_config).GetBlockedFriendAsync(userId, request);
        if (result == null)
            throw new ArgumentException("Error getting blocked friends");
        return result;
    }

    public async Task<GetUsersDTO?> GetUsersAsync(
    int userId,
    GetUsersRequest request)
    {
        var result = await new Data_Access_Layer.FriendRequestsData(_config).GetUsersAsync(userId, request);
        if (result == null)
            throw new ArgumentException("Error getting blocked friends");
        return result;
    }

    public async Task<bool> CheckIfExistFriendship(int currentUserId, int userId)
    {
        return await new Data_Access_Layer.FriendRequestsData(_config).CheckIfExistFriendship(currentUserId, userId);
    }

    public async Task<bool> CreateNewFrienshipAsync(int currentUserId, int userId)
    {
        var res = await new Data_Access_Layer.FriendRequestsData(_config).CreateNewFrienship(currentUserId, userId);
        if (res == true)
        {
            string? userName = await new Data_Access_Layer.UserData(_config).GetUserNameById(currentUserId);
            await new Data_Access_Layer.NotificationsData(_config)
                .NotificateUserAsync(userId, enNotificationType.FriendRequest
                , userName ?? "user");
        }
        return res;
    }
    public async Task<bool> UpdateStateFromRejectedToPending(int currentUserId, int userId)
    {
        return await new Data_Access_Layer.FriendRequestsData(_config).UpdateStateFromRejectedToPending(currentUserId, userId);
    }
    public async Task<GetUsersDTO?> GetUserFriendsAsync(
    int userId,
    GetUsersRequest request, int? myId = null)
    {
        var result = await new Data_Access_Layer.FriendRequestsData(_config).GetUserFriendsAsync(userId, request, myId);
        if (result == null)
            throw new ArgumentException("Error getting user friends");
        return result;
    }

    public async Task<bool> CheckIfIdFromFriendsList(int currentUserId, int userId)
    {
        return await new Data_Access_Layer.FriendRequestsData(_config).
            CheckIfIdFromFriendsList(currentUserId, userId);
    }
}
