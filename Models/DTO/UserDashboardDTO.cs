using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class UserDashboardDTO
{
    public decimal totalAmountSpent { get; set; }
    public decimal savedAmount { get; set; }

    public PurshasedHistoryItem? lastPurshase { get; set; }

    public int purchasedGamesCount { get; set; }
    public int ownedGamesCount { get; set; }
    public int monthPurchasedGamesCount { get; set; }
    public int yearPurchasedGamesCount { get; set; }
    public int installedGamesCount { get; set; }
    public int downloadingGamesCount { get; set; }

    public int friendsCount { get; set; }
    public int pendingFriendsCount { get; set; }

    public List<generalGameDTO>?RecentlyInstalledGames { get; set; }
    public List<generalGameDTO>? RecentlyPurchasedGames { get; set; }
}
