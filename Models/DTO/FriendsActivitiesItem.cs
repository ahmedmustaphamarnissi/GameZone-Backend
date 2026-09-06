using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums;

namespace Models.DTO;

public class FriendsActivitiesItem
{
    public FriendsActivitiesTypes activityType { get; set; }

    public int userId { get; set; }
    public string userName { get; set; } = null!;
    public string? userImage { get; set; }

    public int gameId { get; set; }
    public string gameName { get; set; } = null!;
    public string? gameCover { get; set; }
    public string companyName { get; set; } = null!;
    public string companyCover { get; set; } = null!;
    public List<string>? categories { get; set; }
    public byte? gameRate { get; set; }

    //if the activity type was a review
    public byte? reviewRate { get; set; }
    public string? reviewComment { get; set; }

    public DateTime createdAt { get; set; }
}
