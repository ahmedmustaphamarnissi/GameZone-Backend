using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Data.enums;

public enum enNotificationType
{
    FriendRequest = 1,
    FriendRequestAccepted = 2,
    GameGift = 5,
    PurchaseSuccessful = 6,
    SystemAnnouncement = 13,
    Maintenance = 14,
    Event = 15,

    // V2 / future
    FriendRequestRejected = 3,
    FriendRemoved = 4,
    WishlistDiscount = 7,
    GameUpdate = 8,
    ReviewReply = 9,
    CommentReply = 10,
    CommentMention = 11,
    AchievementUnlocked = 12,
    Promotion = 16,
    SecurityAlert = 17,
    AccountUpdate = 18
}
