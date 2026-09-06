using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameZoneBack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Data.enums;
using Models.DTO;

namespace Data_Access_Layer;

public class NotificationsData : BaseData
{
    public NotificationsData(IConfiguration config) : base(config)
    {
    }

    public async Task<NavbarStateDTO?> GetNavbarStateAsync(int userId)
    {
        using var context = CreateDbContext();

        var userData = await context.Users
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .Select(u => new
            {
                firstName = u.Person.FirstName,
                lastName = u.Person.LastName,
                picturePath = u.Picture.Path,
                lastGlobalNotificationDate = u.LastGlobalNotificationViewedDate
            })
            .FirstOrDefaultAsync();

        if (userData == null)
            return null;

        var userNotificationCount = await context.UserNotifications
            .AsNoTracking()
            .CountAsync(un =>
                un.UserId == userId &&
                !un.IsRead);

        var globalNotificationCount = await context.GlobalNotifications
            .AsNoTracking()
            .CountAsync(gn =>
                userData.lastGlobalNotificationDate == null ||
                gn.CreatedDate > userData.lastGlobalNotificationDate);

        var wishlistCount = await context.WishLists
            .AsNoTracking()
            .CountAsync(w => w.UserId == userId);

        var friendRequestsCount = await context.FriendRequests
            .AsNoTracking()
            .CountAsync(fr =>
                fr.ReceiverId == userId &&
                fr.StatusId == (int)FriendRequestsStatus.Pending);

        var newMessagesCount = await context.Messages
            .AsNoTracking()
            .Where(m =>
                m.ReceiverUserId == userId &&
                !m.IsRead)
            .Select(m => m.SenderUserId)
            .Distinct()
            .CountAsync();

        return new NavbarStateDTO
        {
            firstName = userData.firstName,
            lastName = userData.lastName,
            userPicture = userData.picturePath,
            notificationCount = userNotificationCount + globalNotificationCount,
            wishlistCount = wishlistCount,
            friendRequestsCount = friendRequestsCount,
            newMessagesCount = newMessagesCount
        };
    }

    public async Task<bool?> ReadUserNotificationsAsync(int userId)
    {
        using var context = CreateDbContext();

        var user = await context.Users.FindAsync(userId);

        if (user == null)
            return null;

        var now = DateTime.UtcNow;

        // Mark global notifications as viewed
        user.LastGlobalNotificationViewedDate = now;

        // Mark all unread personal notifications as read
        var userNotifications = await context.UserNotifications
            .Where(un => un.UserId == userId && !un.IsRead)
            .ToListAsync();

        foreach (var notification in userNotifications)
        {
            notification.IsRead = true;
            notification.ReadDate = now;
        }

        await context.SaveChangesAsync();

        return true;
    }
    public async Task<NotificationsResponseDTO?> GetUserNotificationsAsync(
    int userId,
    int pageNumber,
    int pageSize)
    {
        using var context = CreateDbContext();

        // Your existing validation
        if (pageNumber < 1)
            pageNumber = 1;

        if (pageSize < 1)
            pageSize = 10;

        var user = await context.Users.FindAsync(userId);

        if (user == null)
            return null;

        var userNotifications = context.UserNotifications
            .AsNoTracking()
            .Where(un => un.UserId == userId)
            .Select(un => new NotificationItemDTO
            {
                title = un.Title,
                body = un.Body,
                createdDate = un.CreatedDate,
                notificationTypeId = un.NotificationTypeId,
                isRead = un.IsRead
            });

        var globalNotifications = context.GlobalNotifications
            .AsNoTracking()
            .Select(gn => new NotificationItemDTO
            {
                title = gn.Title,
                body = gn.Body,
                createdDate = gn.CreatedDate,
                notificationTypeId = gn.NotificationTypeId,
                isRead = user.LastGlobalNotificationViewedDate != null &&
                         gn.CreatedDate <= user.LastGlobalNotificationViewedDate
            });

        // IMPORTANT: await this before starting another EF operation
        var notifications = await userNotifications
            .Concat(globalNotifications)
            .OrderByDescending(n => n.createdDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var userNotificationCount = await userNotifications.CountAsync();

        var globalNotificationCount = await globalNotifications.CountAsync();

        var notificationCount =
            userNotificationCount + globalNotificationCount;

        return new NotificationsResponseDTO
        {
            totalNotifications = notificationCount,
            notifications = notifications
        };
    }
    public async Task<bool> NotificateUserAsync(
    int userId,
    enNotificationType notificationType,
    string relatedName)
    {
        string title;
        string body;

        switch (notificationType)
        {
            case enNotificationType.FriendRequest:
                title = "New Friend Request";
                body = $"{relatedName} sent you a friend request.";
                break;

            case enNotificationType.FriendRequestAccepted:
                title = "Friend Request Accepted";
                body = $"{relatedName} accepted your friend request.";
                break;

            case enNotificationType.GameGift:
                title = "You Received a Gift";
                body = $"You received {relatedName} as a gift.";
                break;

            case enNotificationType.PurchaseSuccessful:
                title = "Purchase Successful";
                body = $"Your purchase of {relatedName} was successful.";
                break;

            default:
                return false;
        }

        return await CreateUserNotificationAsync(
            userId,
            notificationType,
            title,
            body
        );
    }

    public async Task<bool> CreateUserNotificationAsync(
    int userId,
    enNotificationType notificationType,
    string title,
    string body)
    {
        using var context = CreateDbContext();

        var notification = new UserNotification
        {
            UserId = userId,
            NotificationTypeId = (int)notificationType,
            Title = title,
            Body = body,
            CreatedDate = DateTime.UtcNow,
            IsRead = false,
            ReadDate = null
        };

        await context.UserNotifications.AddAsync(notification);

        var affectedRows = await context.SaveChangesAsync();

        return affectedRows > 0;
    }
}
