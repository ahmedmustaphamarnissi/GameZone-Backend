using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums;

namespace Models.DTO;

public class ProfileDTO
{
    public int userId { get; set; }
    public string userName { get; set; } = null!;
    public string firstName { get; set; } = null!;
    public string lastName { get; set; } = null!;

    public string? countryName { get; set; }
    public string? countryCode { get; set; }

    public string userStatus { get; set; } = null!;
    public string? bio { get; set; }

    public int ownedGamesCount { get; set; }
    public int friendsCount { get; set; }
    public int reviewsCount { get; set; }
    public UsersRelationship relation { get; set; }
    public DateTime? relationUpdateDate { get; set; }
    public List<shortGameDTO>? favoriteGames { get; set; }
    public GetUsersDTO? userFriends { get; set; }

    // Actual reviews
    public List<ReviewDTO>? userReviews { get; set; }

    // Game sections
    public List<shortGameDTO>? reviewedGames { get; set; }
    public List<ProfileShortGamesDTO>? recentlyPurchased { get; set; }
    public List<ProfileShortGamesDTO>? recentlyInWishlist { get; set; }
}
