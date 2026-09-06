using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using GameZoneBack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Data.enums;
using Models.DTO;

namespace Data_Access_Layer;

public class UserData : BaseData
{
    public UserData(IConfiguration config) : base(config) { }

    public async Task<User?> GetUserByUserNameAsync(string username)
    {
        using var context = CreateDbContext();
        var user = await context.Users
            .AsNoTracking()
            .Include(u => u.Person)
                .ThenInclude(p => p.Grade)
            .FirstOrDefaultAsync(u => u.UserName == username);
        return user;
    }

    public async Task<User?> GetUserByUserIdAsync(int userId)
    {
        using var context = CreateDbContext();
        var user = await context.Users
            .AsNoTracking()
            .Include(u => u.Person)
                .ThenInclude(p => p.Grade)
            .FirstOrDefaultAsync(u => u.UserId == userId);
        return user;
    }
    public async Task<User?> AddNewUserAsync(PersonDTO personData, UserDTO UserData)
    {
        using var context = CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var person = new Person
            {
                FirstName = personData.FirstName,
                LastName = personData.LastName,
                Email = personData.Email,
                CountryId = personData.CountryId,
                DateOfBirth = personData.DateOfBirth,
                GradeId = (int)PersonGrade.Customer
            };

            await context.People.AddAsync(person);

            var user = new User
            {
                UserName = UserData.UserName,
                Password = UserData.Password,
                Person = person,
                StatusId = (int)PersonStatus.Active


            };

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            var New = await context.Users
    .Include(u => u.Person)
    .ThenInclude(p => p.Grade)
    .FirstOrDefaultAsync(u => u.UserId == user.UserId);

            return New;

        }
        catch (Exception ex)

        {
            await transaction.RollbackAsync();
            throw;
        }


    }

    public async Task<int?> GetUserIdByEmailAsync(string UserName)
    {
        using var context = CreateDbContext();
        var UserId = await context.Users
    .AsNoTracking()
    .Where(p => p.UserName == UserName)
    .Select(p => (int?)p.UserId)
    .SingleOrDefaultAsync();
        return UserId;
    }
    public async Task<bool> CheckIfUserExistAsync(int Id)
    {
        using var context = CreateDbContext();
        return context.Users.Any(u => u.UserId == Id && u.StatusId == 1);
    }
    public async Task<bool> CheckIfUserProfileExistAsync(int Id)
    {
        using var context = CreateDbContext();
        return context.Users.Any(u => u.UserId == Id &&
        u.Person.Grade.GradeId == (int)PersonGrade.Customer);
    }

    public async Task<bool> CheckIfTheUserBlocked(int Id, int currentUserId)
    {
        using var context = CreateDbContext();
        var data = await context.FriendRequests.Where(fr => (fr.SenderId == currentUserId && fr.ReceiverId == Id)
        || (fr.SenderId == Id && fr.ReceiverId == currentUserId)).FirstOrDefaultAsync();
        if (data == null)
            return false;
        return data.StatusId == (int)FriendRequestsStatus.Blocked;
    }
    private class RelationData
    {
        public int statusId { get; set; }

        public bool IamSender { get; set; }

        public DateTime? Date { get; set; }
    }
    public async Task<ProfileDTO?> GetUserProfile(
    int Id,
    bool IsMyProfile = false,
    int? myId = null)
    {
        using var context = CreateDbContext();

        // =========================
        // User basic information
        // =========================

        var user = await context.Users
            .AsNoTracking()
            .Where(u => u.UserId == Id)
            .Select(res => new
            {
                userId = res.UserId,
                userName = res.UserName,
                firstName = res.Person.FirstName,
                lastName = res.Person.LastName,
                countryName = res.Person.Country.CountryName,
                countryCode = res.Person.Country.CountryCode,
                userStatus = res.Status.StatusName,
                bio = res.Bio,
            })
            .FirstOrDefaultAsync();

        if (user == null)
            return null;


        // =========================
        // Common tasks
        // =========================

        var reviewsTask = RunWithContext(ctx => ctx.Reviews
            .AsNoTracking()
            .Where(r => r.UserId == Id)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDTO
            {
                reviewId = r.ReviewId,
                userName = r.User.UserName,
                gameId = r.GameId,
                userImage = r.User.Picture.Path,
                userId = r.UserId,
                rate = r.Review1,
                reviewComment = r.ReviewComment,
                createdAt = r.CreatedAt,
            })
            .Take(3)
            .ToListAsync());


        // =========================
        // Relationship
        // =========================
        //
        // Only needed when viewing
        // another user's profile.
        //
        // Id  = profile user
        // myId = logged-in user
        //
        // We search specifically for the
        // relationship BETWEEN these users.
        // =========================

        Task<RelationData?> relationTask =
            Task.FromResult<RelationData?>(null);

        if (!IsMyProfile && myId.HasValue && myId.Value != Id)
        {
            relationTask = RunWithContext(ctx =>
                ctx.FriendRequests
                    .AsNoTracking()
                    .Where(fr =>
                        (fr.SenderId == Id &&
                         fr.ReceiverId == myId.Value)
                        ||
                        (fr.SenderId == myId.Value &&
                         fr.ReceiverId == Id))
                    .OrderByDescending(fr =>
                        fr.RespondedDate ?? fr.SendDate)
                    .Select(fr => new RelationData
                    {
                        statusId = fr.StatusId,

                        // Is the logged-in user the sender?
                        IamSender = fr.SenderId == myId.Value,

                        Date = fr.RespondedDate ?? fr.SendDate
                    })
                    .FirstOrDefaultAsync());
        }


        var ownedGamesCountTask = RunWithContext(ctx =>
            ctx.PurchasedGames
                .AsNoTracking()
                .Where(p => p.UserId == Id)
                .CountAsync());


        var friendsCountTask = RunWithContext(ctx =>
            ctx.FriendRequests
                .AsNoTracking()
                .Where(fr =>
                    fr.StatusId == (int)FriendRequestsStatus.Accepted &&
                    (fr.SenderId == Id || fr.ReceiverId == Id))
                .CountAsync());


        var reviewsCountTask = RunWithContext(ctx =>
            ctx.Reviews
                .AsNoTracking()
                .Where(r => r.UserId == Id)
                .CountAsync());


        // =========================
        // Favorite games
        // =========================

        var favoriteGamesTask = RunWithContext(ctx =>
            ctx.Games
                .AsNoTracking()
                .Where(g => g.PurchasedGames.Any(pg =>
                    pg.UserId == Id &&
                    pg.IsFavoriteGame == true))
                .OrderByDescending(g => g.PurchasedGames
                    .Where(pg =>
                        pg.UserId == Id &&
                        pg.IsFavoriteGame == true)
                    .Max(pg => pg.Date))
                .Take(3)
                .Select(g => new shortGameDTO
                {
                    gameId = g.GameId,
                    gameName = g.GameName,

                    gameCover = g.GamesVidsAndPictures
                        .Where(c =>
                            c.IsPrimary == true &&
                            c.Type == false)
                        .Select(c => c.Path)
                        .FirstOrDefault(),

                    price = g.InitialPrice,
                    discount = g.Discount,

                    categories = g.GameGenres
                        .Select(gg => gg.Genre.TypeName)
                        .ToList()
                })
                .ToListAsync());


        // =========================
        // Friends
        // =========================

        Task<GetUsersDTO?> userFriendsTask =
            new FriendRequestsData(_config)
                .GetUserFriendsAsync(
                    Id,
                    new GetUsersRequest
                    {
                        pageNumber = 1,
                        pageSize = 3
                    },
                    myId);


        // =========================
        // My profile only
        // =========================

        Task<List<shortGameDTO>>? reviewedGamesTask = null;

        Task<List<ProfileShortGamesDTO>>? recentlyPurchasedGamesTask = null;

        Task<List<ProfileShortGamesDTO>>? recentlyInWishlistGamesTask = null;


        if (IsMyProfile)
        {
            // =========================
            // Recently reviewed games
            // =========================

            reviewedGamesTask = RunWithContext(ctx =>
                ctx.Games
                    .AsNoTracking()
                    .Where(g => g.Reviews.Any(r => r.UserId == Id))
                    .OrderByDescending(g => g.Reviews
                        .Where(r => r.UserId == Id)
                        .Max(r => r.CreatedAt))
                    .Take(3)
                    .Select(g => new shortGameDTO
                    {
                        gameId = g.GameId,
                        gameName = g.GameName,

                        gameCover = g.GamesVidsAndPictures
                            .Where(c =>
                                c.IsPrimary == true &&
                                c.Type == false)
                            .Select(c => c.Path)
                            .FirstOrDefault(),

                        price = g.InitialPrice,
                        discount = g.Discount,

                        categories = g.GameGenres
                            .Select(gg => gg.Genre.TypeName)
                            .ToList()
                    })
                    .ToListAsync());


            // =========================
            // Recently purchased games
            // =========================

            recentlyPurchasedGamesTask = RunWithContext(ctx =>
                ctx.Games
                    .AsNoTracking()
                    .Where(g => g.PurchasedGames.Any(pg =>
                        pg.UserId == Id))
                    .OrderByDescending(g => g.PurchasedGames
                        .Where(pg => pg.UserId == Id)
                        .Max(pg => pg.Date))
                    .Take(3)
                    .Select(g => new ProfileShortGamesDTO
                    {
                        gameId = g.GameId,
                        gameName = g.GameName,

                        gameCover = g.GamesVidsAndPictures
                            .Where(c =>
                                c.IsPrimary == true &&
                                c.Type == false)
                            .Select(c => c.Path)
                            .FirstOrDefault(),

                        price = g.InitialPrice,
                        discount = g.Discount,

                        categories = g.GameGenres
                            .Select(gg => gg.Genre.TypeName)
                            .ToList(),

                        createdAt = g.PurchasedGames
                            .Where(pg => pg.UserId == Id)
                            .Max(pg => pg.Date)
                    })
                    .ToListAsync());


            // =========================
            // Recently added to wishlist
            // =========================

            recentlyInWishlistGamesTask = RunWithContext(ctx =>
                ctx.Games
                    .AsNoTracking()
                    .Where(g => g.WishLists.Any(w =>
                        w.UserId == Id))
                    .OrderByDescending(g => g.WishLists
                        .Where(w => w.UserId == Id)
                        .Max(w => w.Date))
                    .Take(3)
                    .Select(g => new ProfileShortGamesDTO
                    {
                        gameId = g.GameId,
                        gameName = g.GameName,

                        gameCover = g.GamesVidsAndPictures
                            .Where(c =>
                                c.IsPrimary == true &&
                                c.Type == false)
                            .Select(c => c.Path)
                            .FirstOrDefault(),

                        price = g.InitialPrice,
                        discount = g.Discount,

                        categories = g.GameGenres
                            .Select(gg => gg.Genre.TypeName)
                            .ToList(),

                        createdAt = g.WishLists
                            .Where(w => w.UserId == Id)
                            .Max(w => w.Date)
                    })
                    .ToListAsync());
        }


        // =========================
        // Run all common tasks
        // =========================

        var tasks = new List<Task>
    {
        reviewsTask,
        ownedGamesCountTask,
        friendsCountTask,
        reviewsCountTask,
        favoriteGamesTask,
        userFriendsTask,
        relationTask
    };


        // =========================
        // Add private tasks
        // =========================

        if (IsMyProfile)
        {
            tasks.Add(reviewedGamesTask!);
            tasks.Add(recentlyPurchasedGamesTask!);
            tasks.Add(recentlyInWishlistGamesTask!);
        }


        await Task.WhenAll(tasks);


        // =========================
        // Calculate relationship
        // =========================

        var relation = relationTask.Result;

        UsersRelationship relationship;

        if (relation == null)
        {
            relationship = UsersRelationship.None;
        }
        else if (relation.statusId ==
                 (int)FriendRequestsStatus.Accepted)
        {
            relationship = UsersRelationship.Friend;
        }
        else if (relation.statusId ==
                 (int)FriendRequestsStatus.Pending)
        {
            relationship = relation.IamSender
                ? UsersRelationship.AlreadySend
                : UsersRelationship.Pending;
        }
        else
        {
            // Rejected or any other status.
            //
            // relationUpdateDate is still returned
            // so the caller can check the 24-hour rule.
            relationship = UsersRelationship.None;
        }


        // =========================
        // Return profile
        // =========================

        return new ProfileDTO
        {
            userId = user.userId,
            userName = user.userName,
            firstName = user.firstName,
            lastName = user.lastName,

            countryName = user.countryName,
            countryCode = user.countryCode,

            userStatus = user.userStatus,
            bio = user.bio,

            ownedGamesCount = ownedGamesCountTask.Result,
            friendsCount = friendsCountTask.Result,
            reviewsCount = reviewsCountTask.Result,

            favoriteGames = favoriteGamesTask.Result,

            userFriends = userFriendsTask.Result,

            userReviews = reviewsTask.Result,

            reviewedGames = IsMyProfile
                ? reviewedGamesTask!.Result
                : null,

            recentlyPurchased = IsMyProfile
                ? recentlyPurchasedGamesTask!.Result
                : null,

            recentlyInWishlist = IsMyProfile
                ? recentlyInWishlistGamesTask!.Result
                : null,

            relation = relationship,

            relationUpdateDate = relation?.Date
        };
    }

    public async Task<UserDashboardDTO?> GetUserDashboardAsync(int userId)
    {
        // Check if the user exists
        var userExists = await RunWithContext(async context =>
            await context.Users
                .AsNoTracking()
                .AnyAsync(u => u.UserId == userId));

        if (!userExists)
            return null;

        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var yearStart = new DateTime(now.Year, 1, 1);

        // Purchase statistics
        var purchaseStatsTask = RunWithContext(async context =>
        {
            return await context.PurchasedGames
                .AsNoTracking()
                .Where(pg => pg.UserId == userId)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    TotalAmountSpent = g.Sum(x => x.PurchasedPrice),
                    PurchasedGamesCount = g.Count()
                })
                .FirstOrDefaultAsync();
        });

        // Last purchase
        var lastPurchaseTask = RunWithContext(async context =>
        {
            return await context.PurchasedGames
                .AsNoTracking()
                .Where(pg => pg.UserId == userId)
                .OrderByDescending(pg => pg.Date)
                .Select(pg => new PurshasedHistoryItem
                {
                    PurshasedId = pg.Id,
                    GameId = pg.GameId,
                    GameName = pg.Game.GameName,
                    GameStatus = pg.Game.Status.StatusName,

                    GameCover = pg.Game.GamesVidsAndPictures
                        .Where(gv => gv.IsPrimary && !gv.Type)
                        .Select(gv => gv.Path) // Change Path if your property has another name
                        .FirstOrDefault(),

                    PurshasedDate = pg.Date,
                    PurshasedPrice = pg.PurchasedPrice,
                    PaymentMethod = pg.PaymentMethod.CardType.CardTypeName,
                    IsDefaultCard = pg.PaymentMethod.IsDefault
                })
                .FirstOrDefaultAsync();
        });

        // Recently purchased games
        var recentlyPurchasedTask = RunWithContext(async context =>
        {
            var games = await context.PurchasedGames
                .AsNoTracking()
                .Where(pg => pg.UserId == userId)
                .OrderByDescending(pg => pg.Date)
                .Take(4)
                .Select(pg => new generalGameDTO
                {
                    gameId = pg.GameId,
                    gameName = pg.Game.GameName,

                    gameCover = pg.Game.GamesVidsAndPictures
                        .Where(gv => gv.IsPrimary && !gv.Type)
                        .Select(gv => gv.Path) // Change if needed
                        .FirstOrDefault(),

                    companyId = pg.Game.CompanyId,
                    companyName = pg.Game.Company.CompanyName,
                    companyCover = pg.Game.Company.LogoPath,
                    additionDate = pg.Game.AdditionDate,
                    price = pg.Game.InitialPrice,
                    discount = pg.Game.Discount,
                    AverageRating = pg.Game.Reviews
                        .Select(r => (decimal?)r.Review1)
                        .Average() ?? 0,

                    // Categories are loaded below because List<string>
                    // inside the projection can be problematic depending
                    // on your EF Core model.
                    categories = new List<string>()
                })
                .ToListAsync();

            // Get categories for the selected games
            var gameIds = games.Select(g => g.gameId).ToList();

            var categories = await context.GameGenres
                .AsNoTracking()
                .Where(gg => gameIds.Contains(gg.GameId))
                .GroupBy(gg => gg.GameId)
                .Select(g => new
                {
                    GameId = g.Key,
                    Categories = g.Select(x => x.Genre.TypeName).ToList()
                })
                .ToListAsync();

            foreach (var game in games)
            {
                game.categories = categories
                    .FirstOrDefault(c => c.GameId == game.gameId)?
                    .Categories ?? new List<string>();
            }

            return games;
        });

        // Purchase statistics for current month and year
        var purchasePeriodStatsTask = RunWithContext(async context =>
        {
            return await context.PurchasedGames
                .AsNoTracking()
                .Where(pg => pg.UserId == userId)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    MonthCount = g.Count(x => x.Date >= monthStart),
                    YearCount = g.Count(x => x.Date >= yearStart)
                })
                .FirstOrDefaultAsync();
        });

        // Installation statistics
        var installationStatsTask = RunWithContext(async context =>
        {
            return await context.InstalledGames
                .AsNoTracking()
                .Where(i => i.UserId == userId)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    InstalledGamesCount = g.Count(x => x.IsInstalled == true),

                    DownloadingGamesCount = g.Count(x =>
                        x.IsInstalled == false &&
                        x.IsPaused == false)
                })
                .FirstOrDefaultAsync();
        });

        // Recently installed games
        var recentlyInstalledTask = RunWithContext(async context =>
        {
            var games = await context.InstalledGames
                .AsNoTracking()
                .Where(i => i.UserId == userId && i.IsInstalled == true)
                .OrderByDescending(i => i.UpdatedAt)
                .Take(4)
                .Select(i => new generalGameDTO
                {
                    gameId = i.GameId,
                    gameName = i.Game.GameName,

                    gameCover = i.Game.GamesVidsAndPictures
                        .Where(gv => gv.IsPrimary && !gv.Type)
                        .Select(gv => gv.Path) // Change if needed
                        .FirstOrDefault(),

                    companyId = i.Game.CompanyId,
                    companyName = i.Game.Company.CompanyName,
                    companyCover = i.Game.Company.LogoPath,
                    additionDate = i.Game.AdditionDate,
                    price = i.Game.InitialPrice,
                    discount = i.Game.Discount,
                    AverageRating = i.Game.Reviews
                        .Select(r => (decimal?)r.Review1)
                        .Average() ?? 0,

                    categories = new List<string>()
                })
                .ToListAsync();

            var gameIds = games.Select(g => g.gameId).ToList();

            var categories = await context.GameGenres
                .AsNoTracking()
                .Where(gg => gameIds.Contains(gg.GameId))
                .GroupBy(gg => gg.GameId)
                .Select(g => new
                {
                    GameId = g.Key,
                    Categories = g.Select(x => x.Genre.TypeName).ToList()
                })
                .ToListAsync();

            foreach (var game in games)
            {
                game.categories = categories
                    .FirstOrDefault(c => c.GameId == game.gameId)?
                    .Categories ?? new List<string>();
            }

            return games;
        });

        // Friends statistics
        // Adjust this query according to your actual Friends entity.
        var friendsStatsTask = RunWithContext(async context =>
        {
            return await context.FriendRequests
                .AsNoTracking()
                .Where(f => f.SenderId == userId || f.ReceiverId == userId)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    FriendsCount = g.Count(x =>
                        x.StatusId == (int)FriendRequestsStatus.Accepted),

                    PendingFriendsCount = g.Count(x => x.ReceiverId == userId &&
                        x.StatusId == (int)FriendRequestsStatus.Pending)
                })
                .FirstOrDefaultAsync();
        });

        // Run independent queries in parallel
        await Task.WhenAll(
            purchaseStatsTask,
            lastPurchaseTask,
            recentlyPurchasedTask,
            purchasePeriodStatsTask,
            installationStatsTask,
            recentlyInstalledTask,
            friendsStatsTask
        );

        var purchaseStats = await purchaseStatsTask;
        var periodStats = await purchasePeriodStatsTask;
        var installationStats = await installationStatsTask;
        var friendsStats = await friendsStatsTask;

        return new UserDashboardDTO
        {
            totalAmountSpent = purchaseStats?.TotalAmountSpent ?? 0,

            // You need to calculate this based on the original price
            // and the actual price at the time of purchase.
            savedAmount = 0,

            lastPurshase = await lastPurchaseTask,

            purchasedGamesCount =
                purchaseStats?.PurchasedGamesCount ?? 0,

            // Change this when you support free/claimed games.
            // For now, purchased games = owned games.
            ownedGamesCount =
                purchaseStats?.PurchasedGamesCount ?? 0,

            monthPurchasedGamesCount =
                periodStats?.MonthCount ?? 0,

            yearPurchasedGamesCount =
                periodStats?.YearCount ?? 0,

            installedGamesCount =
                installationStats?.InstalledGamesCount ?? 0,

            downloadingGamesCount =
                installationStats?.DownloadingGamesCount ?? 0,

            friendsCount =
                friendsStats?.FriendsCount ?? 0,

            pendingFriendsCount =
                friendsStats?.PendingFriendsCount ?? 0,

            RecentlyInstalledGames =
                await recentlyInstalledTask,

            RecentlyPurchasedGames =
                await recentlyPurchasedTask
        };
    }
    public async Task<UserProfileSettingsDTO?> GetUserProfileSettingsAsync(int userId)
    {
        using var context = CreateDbContext();

        return await context.People.AsNoTracking().Where(p => p.User.UserId == userId)
            .Select(res => new UserProfileSettingsDTO
            {
                firstName = res.FirstName,
                lastName = res.LastName,
                dateOfBirth = res.DateOfBirth,
                bio = res.User.Bio,
                countryName = res.Country.CountryName,
                countryCode = res.Country.CountryCode,
                email = res.Email,
                phoneNumber = res.PhoneNumber,
                gender = res.Gender,
                userName = res.User.UserName,
                profilePicture = res.User.Picture.Path
            }).FirstOrDefaultAsync();
    }

    public async Task<bool?> InactiveUserAccount(int userId)
    {
        using var context = CreateDbContext();

        var person = await context.Users.FindAsync(userId);

        if (person == null)
            return null;

        if (person.StatusId != (int)PersonStatus.Active)
            throw new ArgumentException("User is not active to implement this operation.");

        person.StatusId = (int)PersonStatus.Inactive;

        return await context.SaveChangesAsync() > 0;
    }


    public async Task<bool?> ChangeUserPassword(int userId, string Password)
    {
        using var context = CreateDbContext();

        var user = await context.Users.FindAsync(userId);

        if (user == null)
            return null;

        user.Password = BCrypt.Net.BCrypt.HashPassword(Password);

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool?> ChangeUserCredentialsAsync(
    int userId,
    UpdateUserDTO request)
    {
        using var context = CreateDbContext();

        var user = await context.Users.Include(u => u.Person).FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return null;

        var countryId = await new Data_Access_Layer.CountriesData(_config)
            .GetCountryIdByCountryCodeAsync(request.countryCode);

        if (!countryId.HasValue)
            return null;

        if (request.gender != null &&
            request.gender != "Male" &&
            request.gender != "Female")
            return null;

        user.Person.FirstName = request.firstName;
        user.Person.LastName = request.lastName;
        user.Person.DateOfBirth = request.dateOfBirth;
        user.Person.CountryId = countryId.Value;
        user.Person.PhoneNumber = request.phoneNumber;
        user.Person.Gender = request.gender;
        user.Bio = request.bio;

        if (request.profilePicture != null)
        {
            if (user.PictureId == null)
            {
                var pic = new UsersPicture
                {
                    Path = request.profilePicture
                };

                await context.UsersPictures.AddAsync(pic);

                user.PictureId = pic.Id;
            }
            else
            {
                var pic = await context.UsersPictures.FindAsync(user.PictureId);

                if (pic == null)
                    return null;

                pic.Path = request.profilePicture;
            }
        }

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<string?> GetUserNameById(int userId)
    {
        using var context = CreateDbContext();
        return await context.Users.Where(u => u.UserId == userId).Select(u => u.UserName)
            .FirstOrDefaultAsync();
    }

    private async Task<T> RunWithContext<T>(Func<GameMarketplaceContext, Task<T>> query)
    {
        using var ctx = CreateDbContext();
        return await query(ctx);
    }

}
