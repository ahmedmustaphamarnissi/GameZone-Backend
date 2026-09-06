using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameZoneBack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Data.enums;
using Models.Data.enums.Sorts;
using Models.DTO;

namespace Data_Access_Layer;

public class FriendRequestsData : BaseData
{
    public FriendRequestsData(IConfiguration config) : base(config)
    {

    }
    public async Task<GetFriendRequestDTO?> GetFriendRequests(
    int userId,
    int pageSize,
    int pageNumber)
    {
        using var context = CreateDbContext();

        var query = context.FriendRequests
            .AsNoTracking()
            .Where(fr =>
                fr.ReceiverId == userId &&
                fr.StatusId == (int)FriendRequestsStatus.Pending)
            .OrderByDescending(fr => fr.SendDate);

        var pendingRequestsCount = await query.CountAsync();

        // precompute current user's friend IDs ONCE instead of a nested correlated EXISTS per row
        var myFriendIds = await context.FriendRequests
            .AsNoTracking()
            .Where(f =>
                f.StatusId == (int)FriendRequestsStatus.Accepted &&
                (f.SenderId == userId || f.ReceiverId == userId))
            .Select(f => f.SenderId == userId ? f.ReceiverId : f.SenderId)
            .ToListAsync();

        var myFriendIdSet = myFriendIds.ToHashSet();

        var pendingRequests = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(fr => new
            {
                fr.SenderId,
                fr.Sender.UserName,
                fr.Sender.Picture.Path,
                fr.SendDate,
                fr.FriendRequestId
            })
            .ToListAsync();

        // mutual friends now computed once per row, in memory, no extra DB round trips
        var senderIds = pendingRequests.Select(r => r.SenderId).ToList();

        var mutualCounts = await context.FriendRequests
            .AsNoTracking()
            .Where(f =>
                f.StatusId == (int)FriendRequestsStatus.Accepted &&
                (senderIds.Contains(f.SenderId) || senderIds.Contains(f.ReceiverId)))
            .Select(f => new { f.SenderId, f.ReceiverId })
            .ToListAsync();

        var friendRequests = pendingRequests.Select(r => new FriendRequestDTO
        {
            userId = r.SenderId,
            userName = r.UserName,
            userImage = r.Path,
            requestDate = r.SendDate,
            friendRequestId = r.FriendRequestId,
            mutualFriendsCount = mutualCounts
                .Where(f => f.SenderId == r.SenderId || f.ReceiverId == r.SenderId)
                .Select(f => f.SenderId == r.SenderId ? f.ReceiverId : f.SenderId)
                .Count(friendId => myFriendIdSet.Contains(friendId))
        }).ToList();

        return new GetFriendRequestDTO { requests = friendRequests, pendingRequestsCount = pendingRequestsCount };
    }

    public async Task<bool> PerformActionOnFriendRequest(int userId, PerformActionOnFriendRequestDTO dto)
    {
        using var context = CreateDbContext();
        var friendRequest = await context.FriendRequests.FindAsync(dto.friendRequestId);
        if (friendRequest == null)
        {
            return false;
        }
        switch (dto.action)
        {
            case FriendRequestActions.Accept:
                friendRequest.StatusId = (int)FriendRequestsStatus.Accepted;
                break;
            case FriendRequestActions.Reject:
                friendRequest.StatusId = (int)FriendRequestsStatus.Rejected;
                break;
            case FriendRequestActions.Block:
                friendRequest.StatusId = (int)FriendRequestsStatus.Blocked;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dto.action), dto.action, null);
        }
        friendRequest.RespondedDate = dto.respondedDate;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<GetFriendsDTO?> GetFriendAsync(
    int userId,
    GetFriendsRequest request)
    {
        using var context = CreateDbContext();

        var query = context.FriendRequests
            .AsNoTracking()
            .Where(fr =>
                (fr.ReceiverId == userId || fr.SenderId == userId) &&
                fr.StatusId == (int)FriendRequestsStatus.Accepted);
        var friendsCount = await query.CountAsync();
        if (!string.IsNullOrEmpty(request.searchText))
        {
            query = query.Where(fr =>
                (fr.Sender.UserName.Contains(request.searchText) && fr.ReceiverId == userId) ||
                (fr.Receiver.UserName.Contains(request.searchText) && fr.SenderId == userId));
        }




        switch (request.orderBy)
        {
            case FriendsOrderBy.UserNameAToZ:
                query = query.OrderBy(fr => fr.SenderId == userId ? fr.Receiver.UserName : fr.Sender.UserName);
                break;
            case FriendsOrderBy.UserNameZToA:
                query = query.OrderByDescending(fr => fr.SenderId == userId ? fr.Receiver.UserName : fr.Sender.UserName);
                break;
            case FriendsOrderBy.RecentlyAdded:
                query = query.OrderByDescending(fr => fr.RespondedDate);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var friendRequests = await query
            .Skip((request.pageNumber - 1) * request.pageSize)
            .Take(request.pageSize)
            .Select(fr => new FriendDto
            {
                userId = fr.SenderId == userId ? fr.ReceiverId : fr.SenderId,
                userName = fr.SenderId == userId ? fr.Receiver.UserName : fr.Sender.UserName,
                userImage = fr.SenderId == userId ? fr.Receiver.Picture.Path : fr.Sender.Picture.Path,
                friendSince = fr.RespondedDate,
                friendRequestId = fr.FriendRequestId,
                roleName = "User", //to fix later
                countryName = fr.SenderId == userId ? fr.Receiver.Person.Country.CountryName : fr.Sender.Person.Country.CountryName,
                countryCode = fr.SenderId == userId ? fr.Receiver.Person.Country.CountryCode : fr.Sender.Person.Country.CountryCode
            })
            .ToListAsync();

        return new GetFriendsDTO { friends = friendRequests, friendsCount = friendsCount };
    }

    public async Task<bool> CheckIfExistFriend(int friendRequestId)
    {
        using var context = CreateDbContext();
        return await context.FriendRequests.AnyAsync(f => f.FriendRequestId == friendRequestId);
    }

    public async Task<bool> CheckIfExistFriendship(int currentUserId, int userId)
    {
        using var context = CreateDbContext();
        return await context.FriendRequests.AnyAsync(f => (f.SenderId == currentUserId && f.ReceiverId == userId) || (f.SenderId == userId && f.ReceiverId == currentUserId));
    }

    public async Task<bool> DeleteFriendAsync(int friendRequestId)
    {
        using var context = CreateDbContext();
        var request = await context.FriendRequests.FindAsync(friendRequestId);
        if (request == null)
            return false;
        context.FriendRequests.Remove(request);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<GetBlockedFriendsDto?> GetBlockedFriendAsync(
    int userId,
    GetFriendsRequest request)
    {
        using var context = CreateDbContext();

        var query = context.FriendRequests
            .AsNoTracking()
            .Where(fr =>
                (fr.ReceiverId == userId || fr.SenderId == userId) &&
                fr.StatusId == (int)FriendRequestsStatus.Blocked);
        var friendsCount = await query.CountAsync();
        if (!string.IsNullOrEmpty(request.searchText))
        {
            query = query.Where(fr =>
                (fr.Sender.UserName.Contains(request.searchText) && fr.ReceiverId == userId) ||
                (fr.Receiver.UserName.Contains(request.searchText) && fr.SenderId == userId));
        }




        switch (request.orderBy)
        {
            case FriendsOrderBy.UserNameAToZ:
                query = query.OrderBy(fr => fr.SenderId == userId ? fr.Receiver.UserName : fr.Sender.UserName);
                break;
            case FriendsOrderBy.UserNameZToA:
                query = query.OrderByDescending(fr => fr.SenderId == userId ? fr.Receiver.UserName : fr.Sender.UserName);
                break;
            case FriendsOrderBy.RecentlyAdded:
                query = query.OrderByDescending(fr => fr.RespondedDate);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var friendRequests = await query
            .Skip((request.pageNumber - 1) * request.pageSize)
            .Take(request.pageSize)
            .Select(fr => new BlockedFriendsDto
            {
                userId = fr.SenderId == userId ? fr.ReceiverId : fr.SenderId,
                userName = fr.SenderId == userId ? fr.Receiver.UserName : fr.Sender.UserName,
                userImage = fr.SenderId == userId ? fr.Receiver.Picture.Path : fr.Sender.Picture.Path,
                blockedSince = fr.RespondedDate,
                friendRequestId = fr.FriendRequestId,

            })
            .ToListAsync();

        return new GetBlockedFriendsDto { blockedFriends = friendRequests, blockedFriendsCount = friendsCount };
    }


    public async Task<GetUsersDTO?> GetUsersAsync(
    int userId,
    GetUsersRequest request)
    {
        using var context = CreateDbContext();

        var query = context.Users
            .AsNoTracking()
            .Where(u =>
                u.UserId != userId && u.StatusId == (int)PersonStatus.Active &&
                !context.FriendRequests.Any(fr =>
                    fr.StatusId == (int)FriendRequestsStatus.Blocked &&
                    (
                        (fr.SenderId == userId && fr.ReceiverId == u.UserId) ||
                        (fr.SenderId == u.UserId && fr.ReceiverId == userId)
                    )));

        if (!string.IsNullOrWhiteSpace(request.searchText))
        {
            query = query.Where(u => u.UserName.Contains(request.searchText));
        }

        var usersCount = await query.CountAsync();

        switch (request.orderBy)
        {
            case UsersOrderBy.UserNameAToZ:
                query = query.OrderBy(u => u.UserName);
                break;

            case UsersOrderBy.UserNameZToA:
                query = query.OrderByDescending(u => u.UserName);
                break;

            case UsersOrderBy.FriendRequestsFirst:
                query = query
                    .OrderByDescending(u =>
                        context.FriendRequests.Any(fr =>
                            fr.SenderId == u.UserId &&
                            fr.ReceiverId == userId &&
                            fr.StatusId == (int)FriendRequestsStatus.Pending))
                    .ThenBy(u => u.UserName);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        var users = await query
            .Skip((request.pageNumber - 1) * request.pageSize)
            .Take(request.pageSize)
            .Select(u => new
            {
                User = u,
                FriendRequest = context.FriendRequests
                    .Where(fr =>
                        (fr.SenderId == u.UserId && fr.ReceiverId == userId) ||
                        (fr.SenderId == userId && fr.ReceiverId == u.UserId))
                    .FirstOrDefault()
            })
            .Select(x => new SearchOnFriendsDTO
            {
                userId = x.User.UserId,
                userName = x.User.UserName,
                userImage = x.User.Picture.Path,

                friendRequestId = x.FriendRequest == null
                    ? null
                    : x.FriendRequest.FriendRequestId,

                relationship =
                    x.FriendRequest == null
                        ? UsersRelationship.None
                        : x.FriendRequest.StatusId == (int)FriendRequestsStatus.Accepted
                            ? UsersRelationship.Friend
                            : x.FriendRequest.SenderId == userId
                                ? UsersRelationship.AlreadySend
                                : UsersRelationship.Pending
            })
            .ToListAsync();

        return new GetUsersDTO
        {
            users = users,
            usersCount = usersCount
        };
    }

    public async Task<GetUsersDTO?> GetUserFriendsAsync(
    int userId,
    GetUsersRequest request, int? myId = null)
    {
        using var context = CreateDbContext();

        var query = context.Users
            .AsNoTracking();
        if (myId == null)
        {
            query = query.Where(u =>
                u.UserId != userId && u.StatusId == (int)PersonStatus.Active &&
                !context.FriendRequests.Any(fr =>
                    fr.StatusId == (int)FriendRequestsStatus.Blocked &&
                    (
                        (fr.SenderId == userId && fr.ReceiverId == u.UserId) ||
                        (fr.SenderId == u.UserId && fr.ReceiverId == userId)
                    )) && context.FriendRequests.Any(fr => ((fr.SenderId == userId && fr.ReceiverId == u.UserId) ||
                        (fr.SenderId == u.UserId && fr.ReceiverId == userId)) && fr.StatusId == (int)FriendRequestsStatus.Accepted));
        }
        else
        {
            query = query.Where(u =>
                u.UserId != userId && u.StatusId == (int)PersonStatus.Active &&
                !context.FriendRequests.Any(fr =>
                    fr.StatusId == (int)FriendRequestsStatus.Blocked &&
                    (
                        (fr.SenderId == myId && fr.ReceiverId == u.UserId) ||
                        (fr.SenderId == u.UserId && fr.ReceiverId == myId)
                    )) && context.FriendRequests.Any(fr => ((fr.SenderId == userId && fr.ReceiverId == u.UserId) ||
                        (fr.SenderId == u.UserId && fr.ReceiverId == userId)) && fr.StatusId == (int)FriendRequestsStatus.Accepted));
        }


        if (!string.IsNullOrWhiteSpace(request.searchText))
        {
            query = query.Where(u => u.UserName.Contains(request.searchText));
        }

        var usersCount = await query.CountAsync();

        switch (request.orderBy)
        {
            case UsersOrderBy.UserNameAToZ:
                query = query.OrderBy(u => u.UserName);
                break;

            case UsersOrderBy.UserNameZToA:
                query = query.OrderByDescending(u => u.UserName);
                break;

            case UsersOrderBy.FriendRequestsFirst:
                query = query
                    .OrderByDescending(u =>
                        context.FriendRequests.Any(fr =>
                            fr.SenderId == u.UserId &&
                            fr.ReceiverId == userId &&
                            fr.StatusId == (int)FriendRequestsStatus.Pending))
                    .ThenBy(u => u.UserName);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        var users = await query
            .Skip((request.pageNumber - 1) * request.pageSize)
            .Take(request.pageSize)
            .Select(u => new
            {
                User = u,
                FriendRequest = context.FriendRequests
                    .Where(fr =>
                        (fr.SenderId == u.UserId && fr.ReceiverId == userId) ||
                        (fr.SenderId == userId && fr.ReceiverId == u.UserId))
                    .FirstOrDefault()
            })
            .Select(x => new SearchOnFriendsDTO
            {
                userId = x.User.UserId,
                userName = x.User.UserName,
                userImage = x.User.Picture.Path,

                friendRequestId = x.FriendRequest == null
                    ? null
                    : x.FriendRequest.FriendRequestId,

                relationship =
                    x.FriendRequest == null
                        ? UsersRelationship.None
                        : x.FriendRequest.StatusId == (int)FriendRequestsStatus.Accepted
                            ? UsersRelationship.Friend
                            : x.FriendRequest.SenderId == userId
                                ? UsersRelationship.AlreadySend
                                : UsersRelationship.Pending
            })
            .ToListAsync();

        return new GetUsersDTO
        {
            users = users,
            usersCount = usersCount
        };
    }

    public async Task<bool> CreateNewFrienship(int currentUserId, int userId)
    {
        using var context = CreateDbContext();
        var friendShip = new FriendRequest
        {
            SenderId = currentUserId,
            ReceiverId = userId,
            StatusId = (int)FriendRequestsStatus.Pending,
            SendDate = DateTime.Now,
        };
        await context.FriendRequests.AddAsync(friendShip);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateStateFromRejectedToPending(int currentUserId, int userId)
    {
        using var context = CreateDbContext();
        var friendShip = await context.FriendRequests.Where(f => (
        (f.SenderId == currentUserId && f.ReceiverId == userId) ||
        (f.SenderId == userId && f.ReceiverId == currentUserId)) && f.StatusId == (int)FriendRequestsStatus.Rejected).FirstOrDefaultAsync();
        if (friendShip == null)
            return false;
        DateTime ResDate = Convert.ToDateTime(friendShip.RespondedDate);

        if ((friendShip.ReceiverId == currentUserId) || ResDate.AddDays(1) <= DateTime.UtcNow)
        {
            friendShip.StatusId = (int)FriendRequestsStatus.Pending;
            friendShip.RespondedDate = DateTime.UtcNow;
            return await context.SaveChangesAsync() > 0;
        }
        return false;
    }


    public async Task<List<SearchOnFriendsDTO>> GetUsersYouMightKhnowAsync(
    int userId)
    {
        using var context = CreateDbContext();
        var rejectedAfter = DateTime.UtcNow.AddHours(-24);

        var query = context.Users
            .AsNoTracking()
            .Where(u =>
                u.UserId != userId &&
                u.StatusId == (int)PersonStatus.Active &&

                !context.FriendRequests.Any(fr =>
                    (
                        (fr.SenderId == userId && fr.ReceiverId == u.UserId) ||
                        (fr.SenderId == u.UserId && fr.ReceiverId == userId)
                    )
                    &&
                    (
                        // Already friends
                        fr.StatusId == (int)FriendRequestsStatus.Accepted

                        // Pending request in either direction
                        || fr.StatusId == (int)FriendRequestsStatus.Pending

                        // Blocked in either direction
                        || fr.StatusId == (int)FriendRequestsStatus.Blocked

                        // Rejected less than 24 hours ago
                        || (
                            fr.StatusId == (int)FriendRequestsStatus.Rejected &&
                            fr.RespondedDate != null &&
                            fr.RespondedDate > rejectedAfter
                        )
                    )
                )
            );




        var users = await query
            .Take(3)
            .Select(u => new
            {
                User = u,
                FriendRequest = context.FriendRequests
                    .Where(fr =>
                        (fr.SenderId == u.UserId && fr.ReceiverId == userId) ||
                        (fr.SenderId == userId && fr.ReceiverId == u.UserId))
                    .FirstOrDefault()
            })
            .Select(x => new SearchOnFriendsDTO
            {
                userId = x.User.UserId,
                userName = x.User.UserName,
                userImage = x.User.Picture.Path,

                friendRequestId = x.FriendRequest == null
                    ? null
                    : x.FriendRequest.FriendRequestId,

                relationship =
                    x.FriendRequest == null
                        ? UsersRelationship.None
                        : x.FriendRequest.StatusId == (int)FriendRequestsStatus.Accepted
                            ? UsersRelationship.Friend
                            : x.FriendRequest.SenderId == userId
                                ? UsersRelationship.AlreadySend
                                : UsersRelationship.Pending
            })
            .ToListAsync();

        return users;
    }

    public async Task<List<FriendsActivitiesItem>> GetFriendsRecentActivitiesAsync(
    int userId,
    int numberToShow = 8)
    {
        using var context = CreateDbContext();

        // ============================================================
        // 1. GET FRIEND IDS
        // ============================================================

        var friendIds = await context.FriendRequests
            .AsNoTracking()
            .Where(x =>
                (x.SenderId == userId || x.ReceiverId == userId) &&
                x.StatusId == (int)FriendRequestsStatus.Accepted
            )
            .Select(x => x.SenderId == userId
                ? x.ReceiverId
                : x.SenderId)
            .ToListAsync();

        if (!friendIds.Any())
            return new List<FriendsActivitiesItem>();


        // ============================================================
        // 2. RECENT PURCHASES
        // ============================================================

        var purchases = await context.PurchasedGames
            .AsNoTracking()
            .Where(x => friendIds.Contains(x.UserId))
            .OrderByDescending(x => x.Date)
            .Take(numberToShow)
            .Select(x => new FriendsActivitiesItem
            {
                activityType = FriendsActivitiesTypes.Purchase,

                userId = x.UserId,
                userName = x.User.UserName,
                userImage = x.User.Picture != null
                    ? x.User.Picture.Path
                    : null,

                gameId = x.GameId,
                gameName = x.Game.GameName,
                gameCover = x.Game.GamesVidsAndPictures
                    .Where(gv => gv.IsPrimary && !gv.Type)
                    .Select(gv => gv.Path)
                    .FirstOrDefault(),

                companyName = x.Game.Company.CompanyName,
                companyCover = x.Game.Company.LogoPath,

                categories = x.Game.GameGenres
                    .Select(g => g.Genre.TypeName)
                    .ToList(),

                createdAt = x.Date
            })
            .ToListAsync();


        // ============================================================
        // 3. RECENT WISHLIST ACTIVITIES
        // ============================================================

        var wishlists = await context.WishLists
            .AsNoTracking()
            .Where(x => friendIds.Contains(x.UserId))
            .OrderByDescending(x => x.Date)
            .Take(numberToShow)
            .Select(x => new FriendsActivitiesItem
            {
                activityType = FriendsActivitiesTypes.Wishlist,

                userId = x.UserId,
                userName = x.User.UserName,
                userImage = x.User.Picture != null
                    ? x.User.Picture.Path
                    : null,

                gameId = x.GameId,
                gameName = x.Game.GameName,
                gameCover = x.Game.GamesVidsAndPictures
                    .Where(gv => gv.IsPrimary && !gv.Type)
                    .Select(gv => gv.Path)
                    .FirstOrDefault(),

                companyName = x.Game.Company.CompanyName,
                companyCover = x.Game.Company.LogoPath,

                categories = x.Game.GameGenres
                    .Select(g => g.Genre.TypeName)
                    .ToList(),

                createdAt = x.Date
            })
            .ToListAsync();


        // ============================================================
        // 4. RECENT REVIEWS
        // ============================================================

        var reviews = await context.Reviews
            .AsNoTracking()
            .Where(x => friendIds.Contains(x.UserId))
            .OrderByDescending(x => x.CreatedAt)
            .Take(numberToShow)
            .Select(x => new FriendsActivitiesItem
            {
                activityType = FriendsActivitiesTypes.Review,

                userId = x.UserId,
                userName = x.User.UserName,
                userImage = x.User.Picture != null
                    ? x.User.Picture.Path
                    : null,

                gameId = x.GameId,
                gameName = x.Game.GameName,
                gameCover = x.Game.GamesVidsAndPictures
                    .Where(gv => gv.IsPrimary && !gv.Type)
                    .Select(gv => gv.Path)
                    .FirstOrDefault(),

                companyName = x.Game.Company.CompanyName,
                companyCover = x.Game.Company.LogoPath,

                categories = x.Game.GameGenres
                    .Select(g => g.Genre.TypeName)
                    .ToList(),
                gameRate = x.Game.Reviews.Any()
    ? (byte?)Math.Round(
        x.Game.Reviews.Average(r => (double)r.Review1)
      )
    : null,
                reviewRate = x.Review1,
                reviewComment = x.ReviewComment,

                createdAt = x.CreatedAt
            })
            .ToListAsync();


        // ============================================================
        // 5. COMBINE IN MEMORY
        // ============================================================

        var activities = purchases
            .Concat(wishlists)
            .Concat(reviews)
            .OrderByDescending(x => x.createdAt)
            .Take(numberToShow)
            .ToList();

        return activities;
    }

    public async Task<SimpleUserData?> GetSenderData(int friendRequestId)
    {
        using var context = CreateDbContext();
        return await context.FriendRequests
            .Where(fr => fr.FriendRequestId == friendRequestId)
            .Select(fr => new SimpleUserData
            {
                UserId = fr.SenderId,
                UserName = fr.Sender.UserName,
            })
            .FirstOrDefaultAsync();
    }
    public async Task<bool> CheckIfIdFromFriendsList(int currentUserId, int userId)
    {
        using var context = CreateDbContext();
        return await context.FriendRequests.AnyAsync(f => (
        (f.SenderId == currentUserId && f.ReceiverId == userId) ||
        (f.SenderId == userId && f.ReceiverId == currentUserId)) &&
        f.StatusId == (int)FriendRequestsStatus.Accepted);
    }
}
