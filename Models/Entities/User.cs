using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class User
{
    public int UserId { get; set; }

    public int PersonId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? PictureId { get; set; }

    public int StatusId { get; set; }

    public DateTime? LastGlobalNotificationViewedDate { get; set; }

    public string? Bio { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<FriendRequest> FriendRequestReceivers { get; set; } = new List<FriendRequest>();

    public virtual ICollection<FriendRequest> FriendRequestSenders { get; set; } = new List<FriendRequest>();

    public virtual ICollection<InstalledGame> InstalledGames { get; set; } = new List<InstalledGame>();

    public virtual ICollection<Message> MessageReceiverUsers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenderUsers { get; set; } = new List<Message>();

    public virtual Person Person { get; set; } = null!;

    public virtual UsersPicture? Picture { get; set; }

    public virtual ICollection<PurchasedGame> PurchasedGames { get; set; } = new List<PurchasedGame>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual PeopleStatus Status { get; set; } = null!;

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();

    public virtual ICollection<WishList> WishLists { get; set; } = new List<WishList>();
}
