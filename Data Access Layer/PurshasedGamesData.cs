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

public class PurshasedGamesData : BaseData
{
    public PurshasedGamesData(IConfiguration config) : base(config)
    {

    }

    public async Task<bool> IsGamePurshased(int GameId, int UserId)
    {
        using var context = CreateDbContext();
        return await context.PurchasedGames.AnyAsync(g => g.UserId == UserId && g.GameId == GameId);
    }

    public async Task<List<LibraryGameItemDTO>?> GetPurchasedGames(int? userId)
    {
        using var context = CreateDbContext();
        var rows = await context.InstalledGames.Where(i => i.UserId == userId &&
             i.IsInstalled != true &&
            i.DownloadedBytes > 0).ToListAsync();
        foreach (var r in rows) r.IsPaused = true;
        await context.SaveChangesAsync();

        return await context.PurchasedGames
            .Where(p => p.UserId == userId)
            .Select(pp => new
            {
                Purchase = pp,
                Installed = context.InstalledGames
                    .FirstOrDefault(ig => ig.GameId == pp.GameId && ig.UserId == userId)
            })
            .Select(x => new LibraryGameItemDTO
            {
                gameId = x.Purchase.GameId,
                gameName = x.Purchase.Game.GameName,

                coverPicture = x.Purchase.Game.GamesVidsAndPictures
                    .Where(vp => vp.IsPrimary)
                    .Select(vp => vp.Path)
                    .FirstOrDefault(),

                isInstalled = x.Installed != null && x.Installed.IsInstalled,
                isPaused = x.Installed != null && x.Installed.IsPaused,
                hasUpdate = x.Installed != null &&
                            x.Installed.InstallationVersion != x.Purchase.Game.GameVersion,
                isFavorite = x.Purchase.IsFavoriteGame
            })
            .ToListAsync();
    }

    public async Task<GameInLibraryDetails?> GetPurchasedGameDetails(int gameId, int? userId)
    {
        using var context = CreateDbContext();




        var game = await context.Games
            .AsNoTracking()
            .Where(g => g.GameId == gameId)
            .Select(res => new
            {
                res.GameId,
                res.GameName,
                res.InitialPrice,
                res.AdditionDate,
                res.Discount,
                res.Size,
                res.GameVersion,
                gameStatus = res.Status.StatusName,
                languages = res.GameLanguages.Select(d => new LanguageDTO
                {
                    languageId = d.Language.LanguageId,
                    languageName = d.Language.LanguageName,
                    languageCode = d.Language.LanguageCode
                }).ToList(),
                features = res.GamesFeatures.Select(d => new FeatureDTO
                {
                    featureId = d.FeatureId,
                    featureName = d.Feature.FeatureName
                }).ToList(),
                updateDate = res.LastUpdate,
                gameDescription = res.GameDescription,
                companyId = res.CompanyId,
                company = new CompaniesDTO
                {
                    companyId = res.Company.CompanyId,
                    companyName = res.Company.CompanyName,
                    countryName = res.Company.Country.CountryName,
                    countryCode = res.Company.Country.CountryCode,
                    companyCover = res.Company.LogoPath,
                    gamesCount = 0
                },
                genreIds = res.GameGenres.Select(gg => gg.GenreId).ToList(),
                categoriesRaw = res.GameGenres.Select(gg => new { gg.Genre.Id, gg.Genre.TypeName }).ToList(),
                media = res.GamesVidsAndPictures
                    .Select(gv => new { gv.Type, gv.IsPrimary, gv.Path })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (game == null) return null;

        var companyGamesCountTask = RunWithContext(ctx =>
            ctx.Games.AsNoTracking().CountAsync(g => g.CompanyId == game.companyId));

        var genreCountsTask = RunWithContext(ctx => ctx.GameGenres.AsNoTracking()
            .Where(gg => game.genreIds.Contains(gg.GenreId))
            .GroupBy(gg => gg.GenreId)
            .Select(g => new { GenreId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.GenreId, x => x.Count));

        var ReviewsCountTask = RunWithContext(ctx => ctx.Reviews.AsNoTracking().Where(r => r.GameId == gameId).CountAsync());

        var PurshasedUserData = RunWithContext(ctx => ctx.PurchasedGames.Where
                                (p => p.UserId == userId && p.GameId == gameId).Select(pp => new
                                {
                                    purchaseDate = pp.Date,
                                    purchasePrice = pp.PurchasedPrice
                                }).FirstOrDefaultAsync());

        var InstalationUserData = RunWithContext(ctx => ctx.InstalledGames.Where
                                (p => p.UserId == userId && p.GameId == gameId).Select(pp => new
                                {
                                    installationId = pp.Id,
                                    installationDate = pp.InstallationDate,
                                    installationVersion = pp.InstallationVersion,
                                    downloadedBytes = pp.DownloadedBytes,
                                    isPaused = pp.IsPaused,
                                    hasUpdate = pp.InstallationVersion != game.GameVersion,
                                    isInstalled = pp.IsInstalled
                                }).FirstOrDefaultAsync());

        var IsFavoriteTask = RunWithContext(ctx => ctx.PurchasedGames.Where
                                (p => p.UserId == userId && p.GameId == gameId)
                                .Select(res => res.IsFavoriteGame).FirstOrDefaultAsync());

        await Task.WhenAll(companyGamesCountTask, genreCountsTask,
            ReviewsCountTask, PurshasedUserData, InstalationUserData, IsFavoriteTask);

        var genreCounts = genreCountsTask.Result;

        var company = game.company;
        company.gamesCount = companyGamesCountTask.Result;

        return new GameInLibraryDetails
        {
            gameId = game.GameId,
            gameName = game.GameName,
            initialPrice = game.InitialPrice,
            additionDate = game.AdditionDate,
            discount = game.Discount,
            size = game.Size,
            gameVersion = game.GameVersion,
            gameStatus = game.gameStatus,
            languages = game.languages,
            features = game.features,
            updateDate = game.updateDate,
            gameDescription = game.gameDescription,
            company = company,
            rating = context.Reviews.Any() ? (decimal)context.Reviews.Average(r => (double)r.Review1) : 0,
            reviewsCount = ReviewsCountTask.Result,
            categories = game.categoriesRaw.Select(c => new CategoriesDTO
            {
                CategoryId = c.Id,
                CategoryName = c.TypeName,
                GamesCount = genreCounts.GetValueOrDefault(c.Id, 0)
            }).ToList(),
            coverPath = game.media.Where(m => m.Type == false && m.IsPrimary == true).Select(m => m.Path).FirstOrDefault(),
            userData = new UserGameInLibraryData
            {
                installationId = InstalationUserData.Result?.installationId,
                purchaseDate = PurshasedUserData.Result?.purchaseDate,
                purchasePrice = PurshasedUserData.Result?.purchasePrice,
                installationDate = InstalationUserData.Result?.installationDate,
                installationVersion = InstalationUserData.Result?.installationVersion,
                downloadedBytes = InstalationUserData.Result?.downloadedBytes,
                isPaused = InstalationUserData.Result?.isPaused ?? false,
                hasUpdate = InstalationUserData.Result?.hasUpdate ?? false,
                isInstalled = InstalationUserData.Result?.isInstalled ?? false,
                isFavorite = IsFavoriteTask.Result
            }
        };
    }

    public async Task<PurshasedHistoryDTO?> GetPurshasedHistoryAsync(
        PurshasedHistoryRequest request, int? userId)
    {
        using var context = CreateDbContext();
        var query = context.PurchasedGames.AsNoTracking()
            .Where(p => p.UserId == userId);
        if(!query.Any()) {
            return null;
        }
        if(request.Filter != null)
        {
            switch(request.Filter)
            { 
                case PurshasedHistoryFilter.Free:
                    query = query.Where(p => p.PurchasedPrice == 0);
                    break;
            }
        }
        if(request.ReleaseDate != null)
        {
            switch (request.ReleaseDate)
            {
                case LongReleaseDate.Last30Days:
                    query = query.Where(p => p.Date >= DateTime.Now.AddMonths(-1));
                    break;
                case LongReleaseDate.Last6Months:
                    query = query.Where(p => p.Date >= DateTime.Now.AddMonths(-6));
                    break;
                case LongReleaseDate.LastYear:
                    query = query.Where(p => p.Date >= DateTime.Now.AddYears(-1));
                    break;
            }
        }

        if(request.OrderBy != null)
        {
            switch (request.OrderBy)
            {
                case PurshasedHistoryOrderBy.OldestFirst:
                    query = query.OrderBy(p => p.Date);
                    break;
                case PurshasedHistoryOrderBy.NewestFirst:
                    query = query.OrderByDescending(p => p.Date);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(p => p.Date);
        }

        var totalCount = await query.CountAsync();
        var totalCosts = await query.SumAsync(p => p.PurchasedPrice);

        var purchases = await query
            .Skip((request.pageNumber - 1) * request.pageSize)
            .Take(request.pageSize)
            .Select(p => new PurshasedHistoryItem
            {
                GameId = p.GameId,
                GameName = p.Game.GameName,
                GameCover = p.Game.GamesVidsAndPictures
                    .Where(vp => vp.IsPrimary && vp.Type == false)
                    .Select(vp => vp.Path)
                    .FirstOrDefault(),
                PurshasedDate = p.Date,
                PurshasedPrice = p.PurchasedPrice,
                GameStatus = p.Game.Status.StatusName,
                PaymentMethod = p.PaymentMethod.CardType.CardTypeName,
                IsDefaultCard = p.PaymentMethod.IsDefault,
                PurshasedId = p.Id

            })
            .ToListAsync();

        return new PurshasedHistoryDTO
        {
            TotalCount = totalCount,
            TotalCost = totalCosts,
            PurshasedHistoryItems = purchases
        };
    }

    public async Task<bool> PerformActionOnFavoriteGame(int userId, int GameId, bool IsToFavorite)
    {
        using var context = CreateDbContext();
        var purchasedGame = await context.PurchasedGames.
            Where(pg => pg.UserId == userId && pg.GameId == GameId).FirstOrDefaultAsync();
        if (purchasedGame == null)
            throw new ArgumentException("purshased recored isn't found");
        purchasedGame.IsFavoriteGame = IsToFavorite;
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool?> PurchaseAGameAsync(int userId, int GameId, int paymentId)
    {
        using var context = CreateDbContext();

        var game = await context.Games
            .Where(g => g.GameId == GameId)
            .Select(res => new
            {
                originalPrice = res.InitialPrice,
                discount = res.Discount,
                eventDiscount = res.EventGames
                    .Select(eg => (decimal?)eg.DiscountPercentage)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        if (game == null)
            return null;

        decimal price;

        if (game.originalPrice == 0)
        {
            price = 0;
        }
        else
        {
            decimal bestDiscount = 0;

            if (game.eventDiscount.HasValue &&
                game.eventDiscount.Value > game.discount &&
                game.eventDiscount.Value > 0)
            {
                bestDiscount = game.eventDiscount.Value;
            }
            else if (game.discount > 0)
            {
                bestDiscount = game.discount;
            }

            price = game.originalPrice * (1 - bestDiscount / 100m);
        }

        var purchaseGame = new PurchasedGame
        {
            UserId = userId,
            GameId = GameId,
            PurchasedPrice = price,
            Date = DateTime.UtcNow,
            PaymentMethodId = paymentId,
            IsFavoriteGame = false
        };

        await context.PurchasedGames.AddAsync(purchaseGame);

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<PurchaseCredentials?> GetUserPurchaseCredentials(
    int userId,
    int gameId)
    {
        using var context = CreateDbContext();

        return await context.PurchasedGames
            .Where(pg => pg.UserId == userId && pg.GameId == gameId)
            .Select(res => new PurchaseCredentials
            {
                UserName = res.User.UserName,
                GameName = res.Game.GameName,
                PurchasePrice = res.PurchasedPrice,
                Email = res.User.Person.Email
            })
            .FirstOrDefaultAsync();
    }
    private async Task<T> RunWithContext<T>(Func<GameMarketplaceContext, Task<T>> query)
    {
        using var ctx = CreateDbContext();
        return await query(ctx);
    }
}

