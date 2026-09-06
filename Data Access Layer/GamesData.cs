using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameZoneBack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;
using Models.Data.enums;
using Models.Data.enums.Sorts;
using Models.DTO;
using Models.DTO.Filter;
using static System.Net.WebRequestMethods;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Data_Access_Layer;

public class GamesData : BaseData
{
    public GamesData(IConfiguration config) : base(config)
    {

    }


    public async Task<List<GameDTO>?> getNewReleasesAsync(int pageNumber, int pageSize)
    {
        using var context = CreateDbContext();
        var data = await context.Games.AsNoTracking().OrderByDescending(g => g.AdditionDate)
    .Skip(pageSize * (pageNumber - 1))
    .Take(pageSize)
    .Select(g => new GameDTO
    {
        gameId = g.GameId,
        gameName = g.GameName,
        price = g.InitialPrice,
        discount = g.Discount,
        gameCover = g.GamesVidsAndPictures
                        .Where(c => c.IsPrimary == true && c.Type == false)
                        .Select(c => c.Path)
                        .FirstOrDefault(),
        companyName = g.Company.CompanyName,
        companyCover = g.Company.LogoPath,
        additionDate = g.AdditionDate,
        status = g.Status.StatusName,
        categories = g.GameGenres.Select(gg => gg.Genre.TypeName).ToList()
    })
    .ToListAsync();
        return data;
    }

    public async Task<List<GameDTO>?> getNewReleasesWithFilterAsync(NewReleasesFilterDTO filter)
    {
        using var context = CreateDbContext();
        var query = context.Games.AsNoTracking();

        if (filter.CategoryIds?.Any() == true)
        {
            query = query.Where(g =>
                g.GameGenres.Any(gg =>
                    filter.CategoryIds.Contains(gg.GenreId)));
        }

        if (filter.featureIds?.Any() == true)
        {
            query = query.Where(g =>
                g.GamesFeatures.Any(gg =>
                    filter.featureIds.Contains(gg.FeatureId)));
        }

        switch (filter.PriceFilter)
        {
            case PriceFilter.Free:
                query = query.Where(g =>
                    g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) == 0);
                break;
            case PriceFilter.Paid:
                query = query.Where(g =>
                    g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) > 0);
                break;
            case PriceFilter.Under10:
                query = query.Where(g =>
                    g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) < 10);
                break;
            case PriceFilter.Under30:
                query = query.Where(g =>
                    g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) < 30);
                break;
            default:
                break;
        }

        switch (filter.ReleasePeriod)
        {
            case ReleasePeriod.Last7Days:
                query = query.Where(g => g.AdditionDate >= DateTime.UtcNow.AddDays(-7));
                break;
            case ReleasePeriod.Last30Days:
                query = query.Where(g => g.AdditionDate >= DateTime.UtcNow.AddDays(-30));
                break;
            case ReleasePeriod.Last90Days:
                query = query.Where(g => g.AdditionDate >= DateTime.UtcNow.AddDays(-90));
                break;
            default:
                break;
        }

        query = filter.OrderBy switch
        {
            NewReleaseOrderBy.OldestFirst =>
                query.OrderBy(g => g.AdditionDate),

            NewReleaseOrderBy.PriceLowToHigh =>
                query.OrderBy(g => g.InitialPrice - (g.InitialPrice * g.Discount / 100)),

            NewReleaseOrderBy.PriceHighToLow =>
                query.OrderByDescending(g => g.InitialPrice - (g.InitialPrice * g.Discount / 100)),

            NewReleaseOrderBy.NameAZ =>
                query.OrderBy(g => g.GameName),

            _ =>
                query.OrderByDescending(g => g.AdditionDate)
        };

        var data = await query
            .Skip(filter.PageSize * (filter.PageNumber - 1))
            .Take(filter.PageSize)
            .Select(g => new GameDTO
            {
                gameId = g.GameId,
                gameName = g.GameName,
                price = g.InitialPrice,
                discount = g.Discount,
                gameCover = g.GamesVidsAndPictures
                                .Where(c => c.IsPrimary == true && c.Type == false)
                                .Select(c => c.Path)
                                .FirstOrDefault(),
                companyName = g.Company.CompanyName,
                companyCover = g.Company.LogoPath,
                additionDate = g.AdditionDate,
                status = g.Status.StatusName,
                categories = g.GameGenres.Select(gg => gg.Genre.TypeName).ToList()
            })
            .ToListAsync();

        return data;
    }

    public async Task<List<shortGameDTO>> getTrendingGames()
    {
        using var context = CreateDbContext();

        var startOfMonth = new DateTime(
            DateTime.Now.Year,
            DateTime.Now.Month,
            1);

        var startOfNextMonth = startOfMonth.AddMonths(1);

        var data = await context.Games
            .AsNoTracking()

            .Select(g => new
            {
                Game = g,

                PurchaseCount = g.PurchasedGames.Count(pg =>
                    pg.Date >= startOfMonth &&
                    pg.Date < startOfNextMonth),

                WishlistCount = g.WishLists.Count(w =>
                    w.Date >= startOfMonth &&
                    w.Date < startOfNextMonth),

                ReviewCount = g.Reviews.Count(r =>
                    r.CreatedAt >= startOfMonth &&
                    r.CreatedAt < startOfNextMonth)
            })

            .OrderByDescending(x =>
                (x.PurchaseCount * 3) +
                (x.WishlistCount * 2) +
                x.ReviewCount)

            .Take(4)

            .Select(x => new shortGameDTO
            {
                gameId = x.Game.GameId,
                gameName = x.Game.GameName,

                gameCover = x.Game.GamesVidsAndPictures
                    .Where(c =>
                        c.IsPrimary == true &&
                        c.Type == false)
                    .Select(c => c.Path)
                    .FirstOrDefault(),

                price = x.Game.InitialPrice,
                discount = x.Game.Discount,

                categories = x.Game.GameGenres
                    .Select(gg => gg.Genre.TypeName)
                    .ToList()
            })

            .ToListAsync();

        return data;
    }

    public async Task<List<GameStoreDTO>?> getTopSellersGamesAsync(int pageNumber, int pageSize)
    {
        using var context = CreateDbContext();
        var data = await context.Games
    .AsNoTracking()
    .Select(g => new GameStoreDTO
    {
        gameId = g.GameId,
        gameName = g.GameName,
        price = g.InitialPrice,
        discount = g.Discount,

        gameCover = g.GamesVidsAndPictures
            .Where(c => c.IsPrimary && !c.Type)
            .Select(c => c.Path)
            .FirstOrDefault(),

        companyName = g.Company.CompanyName,
        companyCover = g.Company.LogoPath,

        additionDate = g.AdditionDate,

        categories = g.GameGenres
            .Select(gg => gg.Genre.TypeName)
            .ToList(),

        AverageRating = g.Reviews.Any()
            ? g.Reviews.Average(r => (decimal)r.Review1)
            : 0,

        ReviewCount = g.Reviews.Count(),

        TotalSold = g.PurchasedGames.Count()
    })
    .OrderByDescending(g => g.TotalSold)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
        return data;
    }

    public async Task<List<GameStoreDTO>?> GetTopSellersGamesWithFiltrationAsync(TopSellersFilter filter)
    {
        using var context = CreateDbContext();
        var query = context.Games.AsNoTracking();

        if (filter.CategoryIds?.Any() == true)
        {
            query = query.Where(g =>
                g.GameGenres.Any(gg =>
                    filter.CategoryIds.Contains(gg.GenreId)));
        }
        query = filter.OrderBy switch
        {
            TopSellersOrderBy.BestSelling =>
                query.OrderByDescending(g => g.PurchasedGames.Count()),

            TopSellersOrderBy.HighestRated =>
                query.OrderByDescending(g => g.Reviews.Any()
                    ? g.Reviews.Average(r => (decimal)r.Review1)
                    : 0),

            TopSellersOrderBy.PriceLowToHigh =>
                query.OrderBy(g => g.InitialPrice - (g.InitialPrice * g.Discount / 100)),

            TopSellersOrderBy.PriceHighToLow =>
                query.OrderByDescending(g => g.InitialPrice - (g.InitialPrice * g.Discount / 100)),

            TopSellersOrderBy.NewestFirst =>
                query.OrderByDescending(g => g.AdditionDate),

            _ =>
                query.OrderByDescending(g => g.AdditionDate)
        };

        var data = await query
    .Select(g => new GameStoreDTO
    {
        gameId = g.GameId,
        gameName = g.GameName,
        price = g.InitialPrice,
        discount = g.Discount,
        gameCover = g.GamesVidsAndPictures
            .Where(c => c.IsPrimary && !c.Type)
            .Select(c => c.Path)
            .FirstOrDefault(),
        companyName = g.Company.CompanyName,
        companyCover = g.Company.LogoPath,
        additionDate = g.AdditionDate,
        categories = g.GameGenres
            .Select(gg => gg.Genre.TypeName)
            .ToList(),
        AverageRating = g.Reviews.Any()
            ? g.Reviews.Average(r => (decimal)r.Review1)
            : 0,
        ReviewCount = g.Reviews.Count(),
        TotalSold = g.PurchasedGames.Count()
    })
    .Skip((filter.PageNumber - 1) * filter.PageSize)
    .Take(filter.PageSize)
    .ToListAsync();
        return data;
    }
    public async Task<List<generalGameDTO>?> GetStoreGamesAsync(StoreCategoryType type, PublicStoreFiltrationRequest? filtration = null)
    {
        using var context = CreateDbContext();
        var query = context.Games
    .AsNoTracking();

        if (filtration != null)
        {
            if (filtration.genresIds?.Any() == true)
            {
                query = query.Where(g =>
                    g.GameGenres.Any(gg =>
                        filtration.genresIds.Contains(gg.GenreId)));
            }

            if (filtration.featuresIds?.Any() == true)
            {
                query = query.Where(g =>
                    g.GamesFeatures.Any(gg =>
                        filtration.featuresIds.Contains(gg.FeatureId)));
            }

            if (filtration.languagesIds?.Any() == true)
            {
                query = query.Where(g =>
                    g.GameLanguages.Any(gg =>
                        filtration.languagesIds.Contains(gg.LanguageId)));
            }

            if (filtration.devicesIds?.Any() == true)
            {
                query = query.Where(g =>
                    g.GameDevices.Any(gg =>
                        filtration.devicesIds.Contains(gg.DeviceId)));
            }

            if (filtration.companiesIds?.Any() == true)
            {
                query = query.Where(g =>
                        filtration.companiesIds.Contains(g.CompanyId));
            }

            switch (filtration.price)
            {
                case PublicStorePriceFiltration.Free:
                    query = query.Where(g =>
                        g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) == 0);
                    break;
                case PublicStorePriceFiltration.Paid:
                    query = query.Where(g =>
                        g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) > 0);
                    break;
                case PublicStorePriceFiltration.Under20:
                    query = query.Where(g =>
                        g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) < 20);
                    break;
                case PublicStorePriceFiltration.Btween20And40:
                    query = query.Where(g =>
                        (g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) < 40) && (g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) >= 20));
                    break;
                case PublicStorePriceFiltration.Btween40And60:
                    query = query.Where(g =>
                        (g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) < 60) && (g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) >= 40));
                    break;
                case PublicStorePriceFiltration.Over60:
                    query = query.Where(g =>
                        (g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) >= 60));
                    break;
                default:
                    break;
            }
        }
        switch (type)
        {
            case StoreCategoryType.freeGames:
                query = query.Where(g => (g.InitialPrice - (g.InitialPrice * (g.Discount / 100))) == 0).OrderByDescending(g => g.AdditionDate);
                break;
            case StoreCategoryType.newReleases:
                query = query.Where(g => g.StatusId == 1).OrderByDescending(g => g.AdditionDate);
                break;
            case StoreCategoryType.topSellers:
                query = query.OrderBy(g => g.PurchasedGames.Count());
                break;
            case StoreCategoryType.specialOffers:
                query = query.Where(g => g.Discount >= 50).OrderByDescending(g => g.Discount);
                break;
            case StoreCategoryType.commingSoon:
                query = query.Where(g => g.Status.StatusId == 3).OrderByDescending(g => g.Discount);
                break;
            default:
                query = query.OrderBy(g => g.AdditionDate);
                break;
        }

        var data = await query
    .Select(g => new generalGameDTO
    {
        gameId = g.GameId,
        gameName = g.GameName,
        price = g.InitialPrice,
        discount = g.Discount,

        gameCover = g.GamesVidsAndPictures
            .Where(c => c.IsPrimary && !c.Type)
            .Select(c => c.Path)
            .FirstOrDefault(),

        companyName = g.Company.CompanyName,
        companyCover = g.Company.LogoPath,

        additionDate = g.AdditionDate,

        categories = g.GameGenres
            .Select(gg => gg.Genre.TypeName)
            .ToList(),

        AverageRating = g.Reviews.Any()
            ? g.Reviews.Average(r => (decimal)r.Review1)
            : 0,

    })
    .Skip(0)
    .Take(4)
    .ToListAsync();
        return data;
    }

    public async Task<List<generalGameDTO>?> GetUserStoreGamesAsync(StoreCategoryType type, int? UserId = null, PublicStoreFiltrationRequest? filtration = null)
    {

        using var context = CreateDbContext();
        var query = context.Games
    .AsNoTracking();

        if (filtration != null)
        {
            if (filtration.genresIds?.Any() == true)
            {
                query = query.Where(g =>
                    g.GameGenres.Any(gg =>
                        filtration.genresIds.Contains(gg.GenreId)));
            }

            if (filtration.featuresIds?.Any() == true)
            {
                query = query.Where(g =>
                    g.GamesFeatures.Any(gg =>
                        filtration.featuresIds.Contains(gg.FeatureId)));
            }

            if (filtration.languagesIds?.Any() == true)
            {
                query = query.Where(g =>
                    g.GameLanguages.Any(gg =>
                        filtration.languagesIds.Contains(gg.LanguageId)));
            }

            if (filtration.devicesIds?.Any() == true)
            {
                query = query.Where(g =>
                    g.GameDevices.Any(gg =>
                        filtration.devicesIds.Contains(gg.DeviceId)));
            }

            if (filtration.companiesIds?.Any() == true)
            {
                query = query.Where(g =>
                        filtration.companiesIds.Contains(g.CompanyId));
            }

            switch (filtration.price)
            {
                case PublicStorePriceFiltration.Free:
                    query = query.Where(g =>
                        g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) == 0);
                    break;
                case PublicStorePriceFiltration.Paid:
                    query = query.Where(g =>
                        g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) > 0);
                    break;
                case PublicStorePriceFiltration.Under20:
                    query = query.Where(g =>
                        g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) < 20);
                    break;
                case PublicStorePriceFiltration.Btween20And40:
                    query = query.Where(g =>
                        (g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) < 40) && (g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) >= 20));
                    break;
                case PublicStorePriceFiltration.Btween40And60:
                    query = query.Where(g =>
                        (g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) < 60) && (g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) >= 40));
                    break;
                case PublicStorePriceFiltration.Over60:
                    query = query.Where(g =>
                        (g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) >= 60));
                    break;
                default:
                    break;
            }
        }

        var ownedGameIds = context.PurchasedGames
.Where(pg => pg.UserId == UserId)
.Select(pg => pg.GameId);

        var purchasedCategories = await context.PurchasedGames
    .Where(pg => pg.UserId == UserId)
    .GroupBy(pg => pg.Game.TypeId)
    .Select(g => new
    {
        CategoryId = g.Key,
        Score = g.Count() * 2 // Give purchases more weight
    })
    .ToListAsync();


        var wishlistCategories = await context.WishLists
    .Where(w => w.UserId == UserId)
    .GroupBy(w => w.Game.TypeId)
    .Select(g => new
    {
        CategoryId = g.Key,
        Score = g.Count()
    })
    .ToListAsync();

        var recommendedCategories = purchasedCategories
    .Concat(wishlistCategories)
    .GroupBy(x => x.CategoryId)
    .Select(g => new
    {
        CategoryId = g.Key,
        Score = g.Sum(x => x.Score)
    })
    .OrderByDescending(x => x.Score)
    .Select(x => x.CategoryId)
    .ToList();

        var purchasedFeatures = await context.PurchasedGames
    .Where(pg => pg.UserId == UserId)
    .SelectMany(pg => pg.Game.GamesFeatures)
    .GroupBy(gf => gf.FeatureId)
    .Select(g => new
    {
        FeatureId = g.Key,
        Score = g.Count() * 2
    })
    .ToListAsync();

        var wishlistFeatures = await context.WishLists
    .Where(w => w.UserId == UserId)
    .SelectMany(w => w.Game.GamesFeatures)
    .GroupBy(gf => gf.FeatureId)
    .Select(g => new
    {
        FeatureId = g.Key,
        Score = g.Count()
    })

    .ToListAsync();


        var recommendedFeatures = purchasedFeatures
    .Concat(wishlistFeatures)
    .GroupBy(x => x.FeatureId)
    .Select(g => new
    {
        FeatureId = g.Key,
        Score = g.Sum(x => x.Score)
    })
    .OrderByDescending(x => x.Score)
    .Select(x => x.FeatureId)
    .ToList();

        switch (type)
        {
            case StoreCategoryType.freeGames:
                query = query.Where(g => (g.InitialPrice - (g.InitialPrice * (g.Discount / 100))) == 0).Where(g => !ownedGameIds.Contains(g.GameId)).OrderByDescending(g => g.AdditionDate);
                break;
            case StoreCategoryType.newReleases:
                query = query.Where(g => g.StatusId == 1).Where(g => !ownedGameIds.Contains(g.GameId)).OrderByDescending(g => g.AdditionDate);
                break;
            case StoreCategoryType.topSellers:
                query = query.Where(g => !ownedGameIds.Contains(g.GameId)).OrderBy(g => g.PurchasedGames.Count());
                break;
            case StoreCategoryType.specialOffers:
                query = query.Where(g => g.Discount >= 50).Where(g => !ownedGameIds.Contains(g.GameId)).OrderByDescending(g => g.Discount);
                break;
            case StoreCategoryType.commingSoon:
                query = query.Where(g => g.Status.StatusId == 3).OrderByDescending(g => g.Discount);
                break;
            case StoreCategoryType.recomended:
                query = query
            .Where(g =>
                !ownedGameIds.Contains(g.GameId) &&
                g.StatusId == 1)
            .OrderByDescending(g =>
                (recommendedCategories.Contains(g.TypeId) ? 10 : 0) +
                (g.GamesFeatures.Count(gf => recommendedFeatures.Contains(gf.FeatureId)) * 3))
            .ThenByDescending(g => g.PurchasedGames.Count());
                break;
            case StoreCategoryType.deals:
                query = query
            .Where(g =>
                !ownedGameIds.Contains(g.GameId) &&
                g.StatusId == 1 && g.Discount >= 40)
            .OrderByDescending(g =>
                (recommendedCategories.Contains(g.TypeId) ? 10 : 0) +
                (g.GamesFeatures.Count(gf => recommendedFeatures.Contains(gf.FeatureId)) * 3))
            .ThenByDescending(g => g.Discount);
                break;
            case StoreCategoryType.wishList:
                query = query
                    .Where(g => context.WishLists.Any(w => w.UserId == UserId && w.GameId == g.GameId))
                    .Where(g => !ownedGameIds.Contains(g.GameId))
                    .OrderByDescending(g => g.PurchasedGames.Count());
                break;
            case StoreCategoryType.updated:
                query = query.OrderByDescending(g => g.LastUpdate);
                break;
            case StoreCategoryType.hidden:
                //later we will change 1 to 10 because we don't have much user in the db 
                query = query.Where(g => !ownedGameIds.Contains(g.GameId))
    .Where(g => g.Reviews.Count() >= 1 &&
                g.Reviews.Average(r => r.Review1) >= 4.5)
    .OrderBy(g => g.PurchasedGames.Count())
    .ThenByDescending(g => g.Reviews.Average(r => r.Review1));
                break;
            default:
                query = query.OrderBy(g => g.AdditionDate);
                break;
        }

        var data = await query
    .Select(g => new generalGameDTO
    {
        gameId = g.GameId,
        gameName = g.GameName,
        price = g.InitialPrice,
        discount = g.Discount,

        gameCover = g.GamesVidsAndPictures
            .Where(c => c.IsPrimary && !c.Type)
            .Select(c => c.Path)
            .FirstOrDefault(),

        companyName = g.Company.CompanyName,
        companyCover = g.Company.LogoPath,

        additionDate = g.AdditionDate,

        categories = g.GameGenres
            .Select(gg => gg.Genre.TypeName)
            .ToList(),

        AverageRating = g.Reviews.Any()
            ? g.Reviews.Average(r => (decimal)r.Review1)
            : 0,

    })
    .Skip(0)
    .Take(5)
    .ToListAsync();
        return data;
    }

    public async Task<StoreEventDTO?> GetStoreEvent(int userId)
    {
        using var context = CreateDbContext();

        var now = DateTime.UtcNow;

        var @event = await context.Events
            .AsNoTracking()
            .Where(e =>
                e.IsActive &&
                e.StartDate <= now &&
                e.EndDate > now)
            .Select(e => new
            {
                e.EventId,
                e.EventName
            })
            .FirstOrDefaultAsync();

        if (@event == null)
            return null;

        var eventGames = await context.EventGames
            .AsNoTracking()
            .Where(eg =>
                eg.EventId == @event.EventId &&
                !context.PurchasedGames.Any(pg =>
                    pg.UserId == userId &&
                    pg.GameId == eg.GameId))
            .OrderByDescending(eg => eg.DiscountPercentage)
            .Select(g => new generalGameDTO
            {
                gameId = g.GameId,
                gameName = g.Game.GameName,
                price = g.Game.InitialPrice,

                discount = Convert.ToInt32(g.DiscountPercentage),

                gameCover = g.Game.GamesVidsAndPictures
                    .Where(c => c.IsPrimary && !c.Type)
                    .Select(c => c.Path)
                    .FirstOrDefault(),

                companyName = g.Game.Company.CompanyName,
                companyCover = g.Game.Company.LogoPath,

                additionDate = g.Game.AdditionDate,

                categories = g.Game.GameGenres
                    .Select(gg => gg.Genre.TypeName)
                    .ToList(),

                AverageRating = g.Game.Reviews.Any()
                    ? g.Game.Reviews.Average(r => (decimal)r.Review1)
                    : 0
            })
            .ToListAsync();

        var maxDiscount = eventGames.Any()
            ? eventGames.Max(g => g.discount)
            : 0;

        return new StoreEventDTO
        {
            eventName = @event.EventName,
            eventGames = eventGames,
            maxDiscount = maxDiscount
        };
    }

    public async Task<List<shortGameDTO>?> GetpublicStoreSearchGames(string search, int pageSize)
    {
        using var context = CreateDbContext();

        var data = await context.Games
            .AsNoTracking().Where(g => g.GameName.Contains(search))
            .OrderByDescending(g => g.PurchasedGames.Count())
            .Take(pageSize)
            .Select(g => new shortGameDTO
            {
                gameId = g.GameId,
                gameName = g.GameName,
                gameCover = g.GamesVidsAndPictures
                                .Where(c => c.IsPrimary == true && c.Type == false)
                                .Select(c => c.Path)
                                .FirstOrDefault(),
                price = g.InitialPrice,
                discount = g.Discount,
            })
            .ToListAsync();
        return data;
    }



    public async Task<List<GameDTO>?> getCommingSoonGamesAsync(int pageNumber, int pageSize)
    {
        using var context = CreateDbContext();
        var data = await context.Games.AsNoTracking().Where(g => g.StatusId == 3).OrderByDescending(g => g.AdditionDate)
    .Skip(pageSize * (pageNumber - 1))
    .Take(pageSize)
    .Select(g => new GameDTO
    {
        gameId = g.GameId,
        gameName = g.GameName,
        price = g.InitialPrice,
        discount = g.Discount,
        gameCover = g.GamesVidsAndPictures
                        .Where(c => c.IsPrimary == true && c.Type == false)
                        .Select(c => c.Path)
                        .FirstOrDefault(),
        companyName = g.Company.CompanyName,
        companyCover = g.Company.LogoPath,
        additionDate = g.AdditionDate,
        status = g.Status.StatusName,
        categories = g.GameGenres.Select(gg => gg.Genre.TypeName).ToList()
    })
    .ToListAsync();
        return data;
    }

    public async Task<List<GameDTO>?> getCommingSoonGamesWithFilterAsync(NewReleasesFilterDTO filter)
    {
        using var context = CreateDbContext();
        var query = context.Games.AsNoTracking().Where(g => g.StatusId == 3);

        if (filter.CategoryIds?.Any() == true)
        {
            query = query.Where(g =>
                g.GameGenres.Any(gg =>
                    filter.CategoryIds.Contains(gg.GenreId)));
        }

        switch (filter.PriceFilter)
        {
            case PriceFilter.Free:
                query = query.Where(g =>
                    g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) == 0);
                break;
            case PriceFilter.Paid:
                query = query.Where(g =>
                    g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) > 0);
                break;
            case PriceFilter.Under10:
                query = query.Where(g =>
                    g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) < 10);
                break;
            case PriceFilter.Under30:
                query = query.Where(g =>
                    g.InitialPrice - ((g.InitialPrice * g.Discount) / 100) < 30);
                break;
            default:
                break;
        }

        switch (filter.ReleasePeriod)
        {
            case ReleasePeriod.Last7Days:
                query = query.Where(g => g.AdditionDate >= DateTime.UtcNow.AddDays(-7));
                break;
            case ReleasePeriod.Last30Days:
                query = query.Where(g => g.AdditionDate >= DateTime.UtcNow.AddDays(-30));
                break;
            case ReleasePeriod.Last90Days:
                query = query.Where(g => g.AdditionDate >= DateTime.UtcNow.AddDays(-90));
                break;
            default:
                break;
        }

        query = filter.OrderBy switch
        {
            NewReleaseOrderBy.OldestFirst =>
                query.OrderBy(g => g.AdditionDate),

            NewReleaseOrderBy.PriceLowToHigh =>
                query.OrderBy(g => g.InitialPrice - (g.InitialPrice * g.Discount / 100)),

            NewReleaseOrderBy.PriceHighToLow =>
                query.OrderByDescending(g => g.InitialPrice - (g.InitialPrice * g.Discount / 100)),

            NewReleaseOrderBy.NameAZ =>
                query.OrderBy(g => g.GameName),

            _ =>
                query.OrderByDescending(g => g.AdditionDate)
        };

        var data = await query
            .Skip(filter.PageSize * (filter.PageNumber - 1))
            .Take(filter.PageSize)
            .Select(g => new GameDTO
            {
                gameId = g.GameId,
                gameName = g.GameName,
                price = g.InitialPrice,
                discount = g.Discount,
                gameCover = g.GamesVidsAndPictures
                                .Where(c => c.IsPrimary == true && c.Type == false)
                                .Select(c => c.Path)
                                .FirstOrDefault(),
                companyName = g.Company.CompanyName,
                companyCover = g.Company.LogoPath,
                additionDate = g.AdditionDate,
                status = g.Status.StatusName,
                categories = g.GameGenres.Select(gg => gg.Genre.TypeName).ToList()
            })
            .ToListAsync();

        return data;
    }
    public async Task<List<GameDTO>?> getFreeGamesWithFilterAsync(FreeGamesFilterDTO filter)
    {
        using var context = CreateDbContext();
        var query = context.Games.AsNoTracking().Where(g => (g.InitialPrice - (g.InitialPrice * g.Discount / 100)) == 0);

        if (filter.CategoryIds?.Any() == true)
        {
            query = query.Where(g =>
                g.GameGenres.Any(gg =>
                    filter.CategoryIds.Contains(gg.GenreId)));
        }


        switch (filter.ReleasePeriod)
        {
            case ReleasePeriod.Last7Days:
                query = query.Where(g => g.AdditionDate >= DateTime.UtcNow.AddDays(-7));
                break;
            case ReleasePeriod.Last30Days:
                query = query.Where(g => g.AdditionDate >= DateTime.UtcNow.AddDays(-30));
                break;
            case ReleasePeriod.Last90Days:
                query = query.Where(g => g.AdditionDate >= DateTime.UtcNow.AddDays(-90));
                break;
            default:
                break;
        }

        query = filter.OrderBy switch
        {
            FreeGamesOrderBy.OldestFirst =>
                query.OrderBy(g => g.AdditionDate),

            FreeGamesOrderBy.BestRatings =>
                query.OrderByDescending(g =>
    g.Reviews.Select(r => (double?)r.Review1).Average() ?? 0),

            FreeGamesOrderBy.NameAZ =>
                query.OrderBy(g => g.GameName),

            _ =>
                query.OrderByDescending(g => g.AdditionDate)
        };

        var data = await query
            .Skip(filter.PageSize * (filter.PageNumber - 1))
            .Take(filter.PageSize)
            .Select(g => new GameDTO
            {
                gameId = g.GameId,
                gameName = g.GameName,
                price = g.InitialPrice,
                discount = g.Discount,
                gameCover = g.GamesVidsAndPictures
                                .Where(c => c.IsPrimary == true && c.Type == false)
                                .Select(c => c.Path)
                                .FirstOrDefault(),
                companyName = g.Company.CompanyName,
                companyCover = g.Company.LogoPath,
                additionDate = g.AdditionDate,
                status = g.Status.StatusName,
                categories = g.GameGenres.Select(gg => gg.Genre.TypeName).ToList()
            })
            .ToListAsync();

        return data;
    }


    public async Task<List<GameStoreDTO>?> GetSpecialOffersGamesWithFiltrationAsync(SpecialOffersFilter filter)
    {
        using var context = CreateDbContext();
        var query = context.Games.AsNoTracking().Where(g => g.Discount >= 40);


        if (filter.CategoryIds?.Any() == true)
        {
            query = query.Where(g =>
                g.GameGenres.Any(gg =>
                    filter.CategoryIds.Contains(gg.GenreId)));
        }
        if (filter.OrderBy != null)
        {
            query = filter.OrderBy switch
            {
                SpecialOffersOrderBy.HighestDiscount =>
                    query.OrderByDescending(g => g.Discount),

                SpecialOffersOrderBy.HighestRated =>
                    query.OrderByDescending(g => g.Reviews.Any()
                        ? g.Reviews.Average(r => (decimal)r.Review1)
                        : 0),

                SpecialOffersOrderBy.PriceLowToHigh =>
                    query.OrderBy(g => g.InitialPrice - (g.InitialPrice * g.Discount / 100)),

                SpecialOffersOrderBy.PriceHighToLow =>
                    query.OrderByDescending(g => g.InitialPrice - (g.InitialPrice * g.Discount / 100)),

                SpecialOffersOrderBy.NewestFirst =>
                    query.OrderByDescending(g => g.AdditionDate),

                _ =>
                    query.OrderByDescending(g => g.AdditionDate)
            };
        }
        else
        {
            query = query.OrderByDescending(g => g.Discount);
        }

        var data = await query
    .Select(g => new GameStoreDTO
    {
        gameId = g.GameId,
        gameName = g.GameName,
        price = g.InitialPrice,
        discount = g.Discount,
        gameCover = g.GamesVidsAndPictures
            .Where(c => c.IsPrimary && !c.Type)
            .Select(c => c.Path)
            .FirstOrDefault(),
        companyName = g.Company.CompanyName,
        companyCover = g.Company.LogoPath,
        additionDate = g.AdditionDate,
        categories = g.GameGenres
            .Select(gg => gg.Genre.TypeName)
            .ToList(),
        AverageRating = g.Reviews.Any()
            ? g.Reviews.Average(r => (decimal)r.Review1)
            : 0,
        ReviewCount = g.Reviews.Count(),
        TotalSold = g.PurchasedGames.Count()
    })
    .Skip((filter.PageNumber - 1) * filter.PageSize)
    .Take(filter.PageSize)
    .ToListAsync();
        return data;
    }

    public async Task<List<GameStoreDTO>> GetGamesByCategorieIdWithFiltrationAsync(
    GamesByCategoriesFiltration filter)
    {
        using var context = CreateDbContext();

        filter.pageNumber = Math.Max(1, filter.pageNumber);
        filter.pageSize = Math.Clamp(filter.pageSize, 1, 50);

        var query = context.Games
            .AsNoTracking()
            .Where(g => g.GameGenres.Any(gg => gg.GenreId == filter.categoryId));

        // Discount Filter
        switch (filter.discount)
        {
            case DiscountChoice.Discount25OrMore:
                query = query.Where(g => g.Discount >= 25);
                break;

            case DiscountChoice.Discount50OrMore:
                query = query.Where(g => g.Discount >= 50);
                break;

            case DiscountChoice.Discount75OrMore:
                query = query.Where(g => g.Discount >= 75);
                break;

            case DiscountChoice.Discount90OrMore:
                query = query.Where(g => g.Discount >= 90);
                break;
        }

        // Price Filter
        if (filter.minPrice >= 0 && filter.maxPrice >= 0)
        {
            query = query.Where(g =>
                (g.InitialPrice - (g.InitialPrice * g.Discount / 100m)) >= filter.minPrice &&
                (g.InitialPrice - (g.InitialPrice * g.Discount / 100m)) <= filter.maxPrice);
        }

        // Release Date Filter
        switch (filter.releaseDate)
        {
            case LongReleaseDate.Last30Days:
                query = query.Where(g =>
                    g.AdditionDate >= DateTime.UtcNow.AddDays(-30));
                break;

            case LongReleaseDate.Last6Months:
                query = query.Where(g =>
                    g.AdditionDate >= DateTime.UtcNow.AddMonths(-6));
                break;

            case LongReleaseDate.LastYear:
                query = query.Where(g =>
                    g.AdditionDate >= DateTime.UtcNow.AddYears(-1));
                break;
        }

        // Rating Filter
        switch (filter.rating)
        {
            case RatingFilter.ThreeStarsAndAbove:
                query = query.Where(g =>
                    g.Reviews.Any() &&
                    g.Reviews.Average(r => r.Review1) >= 3);
                break;

            case RatingFilter.FourStarsAndAbove:
                query = query.Where(g =>
                    g.Reviews.Any() &&
                    g.Reviews.Average(r => r.Review1) >= 4);
                break;

            case RatingFilter.FiveStarsOnly:
                query = query.Where(g =>
                    g.Reviews.Any() &&
                    g.Reviews.Average(r => r.Review1) == 5);
                break;
        }

        // Companies Filter
        if (filter.companiesIds?.Length > 0)
        {
            query = query.Where(g =>
                filter.companiesIds.Contains(g.CompanyId));
        }

        // Devices Filter
        if (filter.DevicesIds?.Length > 0)
        {
            query = query.Where(g =>
                g.GameDevices.Any(d =>
                    filter.DevicesIds.Contains(d.DeviceId)));
        }

        // Features Filter
        if (filter.featureIds?.Length > 0)
        {
            query = query.Where(g =>
                g.GamesFeatures.Any(d =>
                    filter.featureIds.Contains(d.FeatureId)));
        }

        // Language Filter
        if (filter.languageIds?.Length > 0)
        {
            query = query.Where(g =>
                g.GameLanguages.Any(d =>
                    filter.languageIds.Contains(d.LanguageId)));
        }

        // Sorting
        switch (filter.orderBy)
        {
            case GamesByCategoryOrderBy.NameAZ:
                query = query.OrderBy(g => g.GameName);
                break;

            case GamesByCategoryOrderBy.NameZA:
                query = query.OrderByDescending(g => g.GameName);
                break;

            case GamesByCategoryOrderBy.NewestFirst:
                query = query.OrderByDescending(g => g.AdditionDate);
                break;

            case GamesByCategoryOrderBy.OldestFirst:
                query = query.OrderBy(g => g.AdditionDate);
                break;

            case GamesByCategoryOrderBy.PriceLowToHigh:
                query = query.OrderBy(g =>
                    g.InitialPrice - (g.InitialPrice * g.Discount / 100m));
                break;

            case GamesByCategoryOrderBy.PriceHighToLow:
                query = query.OrderByDescending(g =>
                    g.InitialPrice - (g.InitialPrice * g.Discount / 100m));
                break;

            case GamesByCategoryOrderBy.Relevance:
                query = query
                    .OrderByDescending(g => g.PurchasedGames.Count())
                    .ThenByDescending(g => g.Reviews.Any()
                        ? g.Reviews.Average(r => r.Review1)
                        : 0);
                break;

            default:
                query = query.OrderByDescending(g => g.AdditionDate);
                break;
        }

        return await query
            .Select(g => new GameStoreDTO
            {
                gameId = g.GameId,
                gameName = g.GameName,
                price = g.InitialPrice,
                discount = g.Discount,

                gameCover = g.GamesVidsAndPictures
                    .Where(c => c.IsPrimary && !c.Type)
                    .Select(c => c.Path)
                    .FirstOrDefault(),

                companyName = g.Company.CompanyName,
                companyCover = g.Company.LogoPath,
                gameStatus = g.Status.StatusName,

                additionDate = g.AdditionDate,

                categories = g.GameGenres
                    .Select(gg => gg.Genre.TypeName)
                    .ToList(),

                AverageRating = g.Reviews.Any()
                    ? g.Reviews.Average(r => (decimal)r.Review1)
                    : 0,

                ReviewCount = g.Reviews.Count(),

                TotalSold = g.PurchasedGames.Count()
            })
            .Skip((filter.pageNumber - 1) * filter.pageSize)
            .Take(filter.pageSize)
            .ToListAsync();
    }

    public async Task<FiltrationDataOfGamesByCategory> GetGamesByCategoryFiltrations(int Id)
    {
        using var context = CreateDbContext();
        var gamesInCategory = context.Games
    .Where(g => g.GameGenres.Any(gg => gg.GenreId == Id));

        var data = new FiltrationDataOfGamesByCategory
        {
            categoryName = await context.GamesTypes.Where(gt => gt.Id == Id).Select(gt => gt.TypeName).SingleOrDefaultAsync(),
            maxPrice = await gamesInCategory
                .MaxAsync(g => g.InitialPrice - (g.InitialPrice * g.Discount / 100m)),

            minPrice = await gamesInCategory
                .MinAsync(g => g.InitialPrice - (g.InitialPrice * g.Discount / 100m)),

            companies = await context.Companies
                .Where(c => c.Games.Any(g =>
                    g.GameGenres.Any(gg => gg.GenreId == Id))).Select(g => new CompaniesDTO
                    {
                        companyId = g.CompanyId,
                        companyName = g.CompanyName,
                        companyCover = g.LogoPath,
                        gamesCount = g.Games.Count()
                    })
                .ToListAsync(),

            devices = await context.Devices
                .Where(d => d.GameDevices.Any(gd =>
                    gd.Game.GameGenres.Any(gg => gg.GenreId == Id))).Select(d => new DevicesDTO
                    {
                        deviceId = d.DeviceId,
                        deviceName = d.DeviceName,
                        deviceCover = d.IconPath
                    }).ToListAsync(),

            languages = await context.Languages
                .Where(d => d.GameLanguages.Any(gd =>
                    gd.Game.GameGenres.Any(gg => gg.GenreId == Id))).Select(d => new LanguageDTO
                    {
                        languageId = d.LanguageId,
                        languageName = d.LanguageName,
                        languageCode = d.LanguageCode

                    }).ToListAsync(),

            features = await context.Features
                .Where(d => d.GamesFeatures.Any(gd =>
                    gd.Game.GameGenres.Any(gg => gg.GenreId == Id))).Select(d => new FeatureDTO
                    {
                        featureId = d.Id,
                        featureName = d.FeatureName,

                    }).ToListAsync()
        };

        return data;
    }

    public async Task<FiltrationDataOfGamesByPublisher> GetGamesByPublisherFiltrations(int id)
    {
        using var context = CreateDbContext();

        var gamesInPublisher = context.Games.Where(g => g.CompanyId == id);

        var data = new FiltrationDataOfGamesByPublisher
        {
            publisherName = await context.Companies
                .Where(c => c.CompanyId == id)
                .Select(c => c.CompanyName)
                .SingleOrDefaultAsync(),

            maxPrice = await gamesInPublisher
                .MaxAsync(g => g.InitialPrice - (g.InitialPrice * g.Discount / 100m)),

            minPrice = await gamesInPublisher
                .MinAsync(g => g.InitialPrice - (g.InitialPrice * g.Discount / 100m)),

            gameTypes = await gamesInPublisher
                .SelectMany(g => g.GameGenres)
                .GroupBy(gg => new
                {
                    gg.Genre.Id,
                    gg.Genre.TypeName
                })
                .Select(g => new CategoriesDTO
                {
                    CategoryId = g.Key.Id,
                    CategoryName = g.Key.TypeName,
                    GamesCount = g.Count()
                })
                .ToListAsync(),

            devices = await gamesInPublisher
                .SelectMany(g => g.GameDevices)
                .GroupBy(gd => new
                {
                    gd.Device.DeviceId,
                    gd.Device.DeviceName,
                    gd.Device.IconPath
                })
                .Select(g => new DevicesDTO
                {
                    deviceId = g.Key.DeviceId,
                    deviceName = g.Key.DeviceName,
                    deviceCover = g.Key.IconPath
                })
                .ToListAsync(),

            languages = await gamesInPublisher
                .SelectMany(g => g.GameLanguages)
                .GroupBy(gl => new
                {
                    gl.Language.LanguageId,
                    gl.Language.LanguageName,
                    gl.Language.LanguageCode
                })
                .Select(g => new LanguageDTO
                {
                    languageId = g.Key.LanguageId,
                    languageName = g.Key.LanguageName,
                    languageCode = g.Key.LanguageCode
                })
                .ToListAsync(),

            features = await gamesInPublisher
                .SelectMany(g => g.GamesFeatures)
                .GroupBy(gf => new
                {
                    gf.Feature.Id,
                    gf.Feature.FeatureName
                })
                .Select(g => new FeatureDTO
                {
                    featureId = g.Key.Id,
                    featureName = g.Key.FeatureName
                })
                .ToListAsync()
        };

        return data;
    }


    public async Task<List<GameStoreDTO>> GetGamesByPublisherIdWithFiltrationAsync(
    GamesByPublisherFiltration filter)
    {
        using var context = CreateDbContext();

        filter.pageNumber = Math.Max(1, filter.pageNumber);
        filter.pageSize = Math.Clamp(filter.pageSize, 1, 50);

        var query = context.Games
            .AsNoTracking()
            .Where(g => g.CompanyId == filter.publisherId);

        // Discount Filter
        switch (filter.discount)
        {
            case DiscountChoice.Discount25OrMore:
                query = query.Where(g => g.Discount >= 25);
                break;

            case DiscountChoice.Discount50OrMore:
                query = query.Where(g => g.Discount >= 50);
                break;

            case DiscountChoice.Discount75OrMore:
                query = query.Where(g => g.Discount >= 75);
                break;

            case DiscountChoice.Discount90OrMore:
                query = query.Where(g => g.Discount >= 90);
                break;
        }

        // Price Filter
        if (filter.minPrice >= 0 && filter.maxPrice >= 0)
        {
            query = query.Where(g =>
                (g.InitialPrice - (g.InitialPrice * g.Discount / 100m)) >= filter.minPrice &&
                (g.InitialPrice - (g.InitialPrice * g.Discount / 100m)) <= filter.maxPrice);
        }

        // Release Date Filter
        switch (filter.releaseDate)
        {
            case LongReleaseDate.Last30Days:
                query = query.Where(g =>
                    g.AdditionDate >= DateTime.UtcNow.AddDays(-30));
                break;

            case LongReleaseDate.Last6Months:
                query = query.Where(g =>
                    g.AdditionDate >= DateTime.UtcNow.AddMonths(-6));
                break;

            case LongReleaseDate.LastYear:
                query = query.Where(g =>
                    g.AdditionDate >= DateTime.UtcNow.AddYears(-1));
                break;
        }

        // Rating Filter
        switch (filter.rating)
        {
            case RatingFilter.ThreeStarsAndAbove:
                query = query.Where(g =>
                    g.Reviews.Any() &&
                    g.Reviews.Average(r => r.Review1) >= 3);
                break;

            case RatingFilter.FourStarsAndAbove:
                query = query.Where(g =>
                    g.Reviews.Any() &&
                    g.Reviews.Average(r => r.Review1) >= 4);
                break;

            case RatingFilter.FiveStarsOnly:
                query = query.Where(g =>
                    g.Reviews.Any() &&
                    g.Reviews.Average(r => r.Review1) == 5);
                break;
        }

        // Companies Filter
        if (filter.gameTypesIds?.Length > 0)
        {
            query = query.Where(g =>
                g.GameGenres.Any(gg => filter.gameTypesIds.Contains(gg.GenreId)));
        }

        // Devices Filter
        if (filter.DevicesIds?.Length > 0)
        {
            query = query.Where(g =>
                g.GameDevices.Any(d =>
                    filter.DevicesIds.Contains(d.DeviceId)));
        }

        // Companies Filter
        if (filter.featureIds?.Length > 0)
        {
            query = query.Where(g =>
                g.GamesFeatures.Any(gg => filter.gameTypesIds.Contains(gg.FeatureId)));
        }

        // Devices Filter
        if (filter.languageIds?.Length > 0)
        {
            query = query.Where(g =>
                g.GameLanguages.Any(d =>
                    filter.languageIds.Contains(d.LanguageId)));
        }

        // Sorting
        switch (filter.orderBy)
        {
            case GamesByCategoryOrderBy.NameAZ:
                query = query.OrderBy(g => g.GameName);
                break;

            case GamesByCategoryOrderBy.NameZA:
                query = query.OrderByDescending(g => g.GameName);
                break;

            case GamesByCategoryOrderBy.NewestFirst:
                query = query.OrderByDescending(g => g.AdditionDate);
                break;

            case GamesByCategoryOrderBy.OldestFirst:
                query = query.OrderBy(g => g.AdditionDate);
                break;

            case GamesByCategoryOrderBy.PriceLowToHigh:
                query = query.OrderBy(g =>
                    g.InitialPrice - (g.InitialPrice * g.Discount / 100m));
                break;

            case GamesByCategoryOrderBy.PriceHighToLow:
                query = query.OrderByDescending(g =>
                    g.InitialPrice - (g.InitialPrice * g.Discount / 100m));
                break;

            case GamesByCategoryOrderBy.Relevance:
                query = query
                    .OrderByDescending(g => g.PurchasedGames.Count())
                    .ThenByDescending(g => g.Reviews.Any()
                        ? g.Reviews.Average(r => r.Review1)
                        : 0);
                break;

            default:
                query = query.OrderByDescending(g => g.AdditionDate);
                break;
        }

        return await query
            .Select(g => new GameStoreDTO
            {
                gameId = g.GameId,
                gameName = g.GameName,
                price = g.InitialPrice,
                discount = g.Discount,

                gameCover = g.GamesVidsAndPictures
                    .Where(c => c.IsPrimary && !c.Type)
                    .Select(c => c.Path)
                    .FirstOrDefault(),

                companyName = g.Company.CompanyName,
                companyCover = g.Company.LogoPath,
                gameStatus = g.Status.StatusName,

                additionDate = g.AdditionDate,

                categories = g.GameGenres
                    .Select(gg => gg.Genre.TypeName)
                    .ToList(),

                AverageRating = g.Reviews.Any()
                    ? g.Reviews.Average(r => (decimal)r.Review1)
                    : 0,

                ReviewCount = g.Reviews.Count(),

                TotalSold = g.PurchasedGames.Count()
            })
            .Skip((filter.pageNumber - 1) * filter.pageSize)
            .Take(filter.pageSize)
            .ToListAsync();
    }

    public async Task<bool> CheckIfGameExist(int Id)
    {
        using var context = CreateDbContext();
        return await context.Games.AnyAsync(g => g.GameId == Id);
    }

    public async Task<GameHoverDTO?> GetHoverOnGameDataAsync(int Id)
    {
        using var context = CreateDbContext();

        var data = await context.Games.Where(g => g.GameId == Id).Select(res => new GameHoverDTO
        {
            gameId = res.GameId,
            title = res.GameName,
            lastUpdateDate = res.LastUpdate,
            shortDescription = res.GameDescription,
            rating = res.Reviews.Any()
                    ? res.Reviews.Average(r => (decimal)r.Review1)
                    : 0,
            reviewsCount = res.Reviews.Count(),
            genres = res.GameGenres.Select(gg => gg.Genre.TypeName).ToList(),
            trailer = res.GamesVidsAndPictures.Where(gv => gv.Type == true).Select(r => r.Path).FirstOrDefault(),
            screenshots = res.GamesVidsAndPictures.Where(gv => gv.Type == false).Select(r => r.Path).ToList()

        }).FirstOrDefaultAsync();

        return data;
    }


    public async Task<GameDetailsDTO?> GetGameDetailsDataAsync(int Id, int? UserId = null)
    {
        using var mainContext = CreateDbContext();

        var game = await mainContext.Games
            .AsNoTracking()
            .Where(g => g.GameId == Id)
            .Select(res => new
            {
                res.GameId,
                res.GameName,
                res.InitialPrice,
                res.AdditionDate,
                res.Discount,
                res.AppropriateAge,
                res.Size,
                res.GameVersion,
                gameStatus = res.Status.StatusName,
                devices = res.GameDevices.Select(d => new DevicesDTO
                {
                    deviceId = d.Device.DeviceId,
                    deviceName = d.Device.DeviceName,
                    deviceCover = d.Device.IconPath
                }).ToList(),
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

        // Each of these gets its own DbContext instance, so they're safe to run in parallel
        var reviewsTask = RunWithContext(ctx => ctx.Reviews.AsNoTracking()
            .Where(r => r.GameId == Id && r.UserId != UserId).OrderByDescending(rr => rr.CreatedAt)
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
            }).Take(3).ToListAsync());

        var commentsTask = RunWithContext(ctx => ctx.Comments.AsNoTracking()
            .Where(c => c.GameId == Id).OrderByDescending(rr => rr.CommentDate)
            .Select(c => new CommentDTO
            {
                commentId = c.CommentId,
                commentDate = c.CommentDate,
                commentText = c.CommentText,
                userName = c.User.UserName,
                userImage = c.User.Picture.Path,
                userId = c.UserId,
            }).Take(3).ToListAsync());

        var similarGamesTask = RunWithContext(ctx => ctx.Games.AsNoTracking()
            .Where(g => g.GameId != Id && g.GameGenres.Any(gg => game.genreIds.Contains(gg.GenreId)))
            .OrderByDescending(g => g.PurchasedGames.Count())
            .Take(3)
            .Select(g => new shortGameDTO
            {
                gameId = g.GameId,
                gameName = g.GameName,
                gameCover = g.GamesVidsAndPictures
                    .Where(c => c.IsPrimary == true && c.Type == false)
                    .Select(c => c.Path).FirstOrDefault(),
                price = g.InitialPrice,
                discount = g.Discount,
                categories = g.GameGenres.Select(gg => gg.Genre.TypeName).ToList()
            }).ToListAsync());

        var companyGamesTask = RunWithContext(ctx => ctx.Games.AsNoTracking()
            .Where(g => g.CompanyId == game.companyId && g.GameId != Id)
            .OrderByDescending(g => g.PurchasedGames.Count())
            .Take(3)
            .Select(g => new shortGameDTO
            {
                gameId = g.GameId,
                gameName = g.GameName,
                gameCover = g.GamesVidsAndPictures
                    .Where(c => c.IsPrimary == true && c.Type == false)
                    .Select(c => c.Path).FirstOrDefault(),
                price = g.InitialPrice,
                discount = g.Discount,
                categories = g.GameGenres.Select(gg => gg.Genre.TypeName).ToList()
            }).ToListAsync());

        var minimumRecTask = RunWithContext(ctx => ctx.SystemRequirements.AsNoTracking().
        Where(sys => sys.RequirementTypeId == 1 && sys.GameId == Id).Select(s => new SystemRequirementDTO
        {
            SystemRequirementID = s.SystemRequirementId,
            GameID = s.GameId,
            DeviceName = s.Device.DeviceName,
            RequirementType = s.RequirementType.RequirementTypeName,
            OperatingSystem = s.OperatingSystem,
            AdditionalNotes = s.AdditionalNotes,
            Network = s.Network,
            SoundCard = s.SoundCard,
            Storage = s.Storage,
            DirectX = s.DirectX,
            Graphics = s.Graphics,
            Memory = s.Memory,
            Processor = s.Processor,
        }).FirstOrDefaultAsync());

        var recommendedRecTask = RunWithContext(ctx => ctx.SystemRequirements.AsNoTracking().
        Where(sys => sys.RequirementTypeId == 2 && sys.GameId == Id).Select(s => new SystemRequirementDTO
        {
            SystemRequirementID = s.SystemRequirementId,
            GameID = s.GameId,
            DeviceName = s.Device.DeviceName,
            RequirementType = s.RequirementType.RequirementTypeName,
            OperatingSystem = s.OperatingSystem,
            AdditionalNotes = s.AdditionalNotes,
            Network = s.Network,
            SoundCard = s.SoundCard,
            Storage = s.Storage,
            DirectX = s.DirectX,
            Graphics = s.Graphics,
            Memory = s.Memory,
            Processor = s.Processor,
        }).FirstOrDefaultAsync());

        var companyGamesCountTask = RunWithContext(ctx =>
            ctx.Games.AsNoTracking().CountAsync(g => g.CompanyId == game.companyId));

        var genreCountsTask = RunWithContext(ctx => ctx.GameGenres.AsNoTracking()
            .Where(gg => game.genreIds.Contains(gg.GenreId))
            .GroupBy(gg => gg.GenreId)
            .Select(g => new { GenreId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.GenreId, x => x.Count));

        var ReviewsCountTask = RunWithContext(ctx => ctx.Reviews.AsNoTracking().Where(r => r.GameId == Id).CountAsync());
        var CommentCountTask = RunWithContext(ctx => ctx.Comments.AsNoTracking().Where(r => r.GameId == Id).CountAsync());

        var ReviewsDashboardTask = RunWithContext(ctx =>
        ctx.Reviews
        .AsNoTracking()
        .Where(r => r.GameId == Id)
        .GroupBy(_ => 1)
        .Select(g => new ReviewsDashboardDTO
        {
            FiveStarCount = g.Count(r => r.Review1 == 5),
            FourStarCount = g.Count(r => r.Review1 == 4),
            ThreeStarCount = g.Count(r => r.Review1 == 3),
            TwoStarCount = g.Count(r => r.Review1 == 2),
            OneStarCount = g.Count(r => r.Review1 == 1)
        })
        .FirstOrDefaultAsync());

        await Task.WhenAll(
            reviewsTask, commentsTask, similarGamesTask,
            companyGamesTask, companyGamesCountTask, genreCountsTask, minimumRecTask
            , recommendedRecTask, ReviewsCountTask, CommentCountTask, ReviewsDashboardTask);

        var reviews = reviewsTask.Result;
        var genreCounts = genreCountsTask.Result;

        var company = game.company;
        company.gamesCount = companyGamesCountTask.Result;

        return new GameDetailsDTO
        {
            gameId = game.GameId,
            gameName = game.GameName,
            initialPrice = game.InitialPrice,
            additionDate = game.AdditionDate,
            discount = game.Discount,
            appropriateAge = game.AppropriateAge,
            size = game.Size,
            gameVersion = game.GameVersion,
            gameStatus = game.gameStatus,
            devices = game.devices,
            languages = game.languages,
            features = game.features,
            updateDate = game.updateDate,
            gameDescription = game.gameDescription,
            commentsCount = CommentCountTask.Result,
            company = company,
            reviews = reviews,
            reviewsDashboard = ReviewsDashboardTask.Result,
            comments = commentsTask.Result,
            similarGames = similarGamesTask.Result,
            companyGames = companyGamesTask.Result,
            rating = reviews.Any() ? (decimal)reviews.Average(r => (double)r.rate) : 0,
            reviewsCount = ReviewsCountTask.Result,
            minimumSystemRequirements = minimumRecTask.Result,
            recommendedSystemRequirements = recommendedRecTask.Result,
            categories = game.categoriesRaw.Select(c => new CategoriesDTO
            {
                CategoryId = c.Id,
                CategoryName = c.TypeName,
                GamesCount = genreCounts.GetValueOrDefault(c.Id, 0)
            }).ToList(),
            trailerLink = game.media.Where(m => m.Type == true).Select(m => m.Path).FirstOrDefault(),
            screenshotsPaths = game.media.Where(m => m.Type == false && m.IsPrimary == false).Select(m => m.Path).ToList(),
            coverPath = game.media.Where(m => m.Type == false && m.IsPrimary == true).Select(m => m.Path).FirstOrDefault(),
            userData = await GetUserGameDataAsync(Id, UserId)
        };
    }

    public async Task<UserGameData?> GetUserGameDataAsync(int GameId, int? UserId)
    {
        if (UserId == null || UserId == 0)
            return null;
        var context = CreateDbContext();
        var query = await context.Games.Where(g => g.GameId == GameId).Select(u => new UserGameData
        {
            isInWishlist = u.WishLists.Any(w => w.UserId == UserId),
            ownsGame = u.PurchasedGames.Any(w => w.UserId == UserId),
            userCommentsIds = u.Comments.Where(cc => cc.UserId == UserId && cc.GameId == GameId).Select(c => c.CommentId)
            .ToArray(),

            userReview = context.Reviews.Where(r => r.UserId == UserId && r.GameId == GameId).Select(r => new ReviewDTO
            {
                reviewId = r.ReviewId,
                userName = r.User.UserName,
                gameId = r.GameId,
                userImage = r.User.Picture.Path,
                userId = r.UserId,
                rate = r.Review1,
                reviewComment = r.ReviewComment,
                createdAt = r.CreatedAt,
            }).FirstOrDefault(),
        }).FirstOrDefaultAsync();
        return query;
    }


    public async Task<List<GameStoreDTO>> GetGamesBySectionWithFiltrationAsync(
    GamesBySectionFiltrations filter, int? UserId)
    {
        using var context = CreateDbContext();

        filter.pageNumber = Math.Max(1, filter.pageNumber);
        filter.pageSize = Math.Clamp(filter.pageSize, 1, 50);

        var query = context.Games
            .AsNoTracking()
            .Where(g => g.StatusId == 1);




        // Discount Filter
        switch (filter.discount)
        {
            case DiscountChoice.Discount25OrMore:
                query = query.Where(g => g.Discount >= 25);
                break;

            case DiscountChoice.Discount50OrMore:
                query = query.Where(g => g.Discount >= 50);
                break;

            case DiscountChoice.Discount75OrMore:
                query = query.Where(g => g.Discount >= 75);
                break;

            case DiscountChoice.Discount90OrMore:
                query = query.Where(g => g.Discount >= 90);
                break;
        }

        // Price Filter
        if (filter.minPrice >= 0 && filter.maxPrice >= 0)
        {
            query = query.Where(g =>
                (g.InitialPrice - (g.InitialPrice * g.Discount / 100m)) >= filter.minPrice &&
                (g.InitialPrice - (g.InitialPrice * g.Discount / 100m)) <= filter.maxPrice);
        }

        // Release Date Filter
        switch (filter.releaseDate)
        {
            case LongReleaseDate.Last30Days:
                query = query.Where(g =>
                    g.AdditionDate >= DateTime.UtcNow.AddDays(-30));
                break;

            case LongReleaseDate.Last6Months:
                query = query.Where(g =>
                    g.AdditionDate >= DateTime.UtcNow.AddMonths(-6));
                break;

            case LongReleaseDate.LastYear:
                query = query.Where(g =>
                    g.AdditionDate >= DateTime.UtcNow.AddYears(-1));
                break;
        }

        // Rating Filter
        switch (filter.rating)
        {
            case RatingFilter.ThreeStarsAndAbove:
                query = query.Where(g =>
                    g.Reviews.Any() &&
                    g.Reviews.Average(r => r.Review1) >= 3);
                break;

            case RatingFilter.FourStarsAndAbove:
                query = query.Where(g =>
                    g.Reviews.Any() &&
                    g.Reviews.Average(r => r.Review1) >= 4);
                break;

            case RatingFilter.FiveStarsOnly:
                query = query.Where(g =>
                    g.Reviews.Any() &&
                    g.Reviews.Average(r => r.Review1) == 5);
                break;
        }

        // Companies Filter
        if (filter.companiesIds?.Length > 0)
        {
            query = query.Where(g =>
                filter.companiesIds.Contains(g.CompanyId));
        }

        // Devices Filter
        if (filter.DevicesIds?.Length > 0)
        {
            query = query.Where(g =>
                g.GameDevices.Any(d =>
                    filter.DevicesIds.Contains(d.DeviceId)));
        }

        // Features Filter
        if (filter.featureIds?.Length > 0)
        {
            query = query.Where(g =>
                g.GamesFeatures.Any(d =>
                    filter.featureIds.Contains(d.FeatureId)));
        }

        // Language Filter
        if (filter.languageIds?.Length > 0)
        {
            query = query.Where(g =>
                g.GameLanguages.Any(d =>
                    filter.languageIds.Contains(d.LanguageId)));
        }
        // Genre Filter
        if (filter.categoryIds?.Length > 0)
        {
            query = query.Where(g =>
                g.GameGenres.Any(d =>
                    filter.categoryIds.Contains(d.GenreId)));
        }

        var ownedGameIds = context.PurchasedGames
.Where(pg => pg.UserId == UserId)
.Select(pg => pg.GameId);

        var purchasedCategories = await context.PurchasedGames
    .Where(pg => pg.UserId == UserId)
    .GroupBy(pg => pg.Game.TypeId)
    .Select(g => new
    {
        CategoryId = g.Key,
        Score = g.Count() * 2 // Give purchases more weight
    })
    .ToListAsync();


        var wishlistCategories = await context.WishLists
    .Where(w => w.UserId == UserId)
    .GroupBy(w => w.Game.TypeId)
    .Select(g => new
    {
        CategoryId = g.Key,
        Score = g.Count()
    })
    .ToListAsync();

        var recommendedCategories = purchasedCategories
    .Concat(wishlistCategories)
    .GroupBy(x => x.CategoryId)
    .Select(g => new
    {
        CategoryId = g.Key,
        Score = g.Sum(x => x.Score)
    })
    .OrderByDescending(x => x.Score)
    .Select(x => x.CategoryId)
    .ToList();

        var purchasedFeatures = await context.PurchasedGames
    .Where(pg => pg.UserId == UserId)
    .SelectMany(pg => pg.Game.GamesFeatures)
    .GroupBy(gf => gf.FeatureId)
    .Select(g => new
    {
        FeatureId = g.Key,
        Score = g.Count() * 2
    })
    .ToListAsync();

        var wishlistFeatures = await context.WishLists
    .Where(w => w.UserId == UserId)
    .SelectMany(w => w.Game.GamesFeatures)
    .GroupBy(gf => gf.FeatureId)
    .Select(g => new
    {
        FeatureId = g.Key,
        Score = g.Count()
    })

    .ToListAsync();


        var recommendedFeatures = purchasedFeatures
    .Concat(wishlistFeatures)
    .GroupBy(x => x.FeatureId)
    .Select(g => new
    {
        FeatureId = g.Key,
        Score = g.Sum(x => x.Score)
    })
    .OrderByDescending(x => x.Score)
    .Select(x => x.FeatureId)
    .ToList();

        switch (filter.sectionType)
        {
            case UserStoreSectionType.HiddenGems:
                // later we will change 0 to 10
                query = query.Where(g => !ownedGameIds.Contains(g.GameId))
    .Where(g => g.Reviews.Count() >= 0 &&
                g.Reviews.Average(r => r.Review1) >= 4.5)
    .OrderBy(g => g.PurchasedGames.Count())
    .ThenByDescending(g => g.Reviews.Average(r => r.Review1));
                break;

            case UserStoreSectionType.RecentlyUpdatedGames:

                query = query.OrderByDescending(g => g.LastUpdate);
                break;

            case UserStoreSectionType.RecommendedForYou:

                query = query
            .Where(g =>
                !ownedGameIds.Contains(g.GameId) &&
                g.StatusId == 1)
            .OrderByDescending(g =>
                (recommendedCategories.Contains(g.TypeId) ? 10 : 0) +
                (g.GamesFeatures.Count(gf => recommendedFeatures.Contains(gf.FeatureId)) * 3))
            .ThenByDescending(g => g.PurchasedGames.Count());
                break;

            case UserStoreSectionType.DealsForYou:
                query = query
            .Where(g =>
                !ownedGameIds.Contains(g.GameId) &&
                g.StatusId == 1 && g.Discount >= 40)
            .OrderByDescending(g =>
                (recommendedCategories.Contains(g.TypeId) ? 10 : 0) +
                (g.GamesFeatures.Count(gf => recommendedFeatures.Contains(gf.FeatureId)) * 3))
            .ThenByDescending(g => g.Discount);
                break;
        }

        // Sorting
        switch (filter.orderBy)
        {
            case GamesByCategoryOrderBy.NameAZ:
                query = query.OrderBy(g => g.GameName);
                break;

            case GamesByCategoryOrderBy.NameZA:
                query = query.OrderByDescending(g => g.GameName);
                break;

            case GamesByCategoryOrderBy.NewestFirst:
                query = query.OrderByDescending(g => g.AdditionDate);
                break;

            case GamesByCategoryOrderBy.OldestFirst:
                query = query.OrderBy(g => g.AdditionDate);
                break;

            case GamesByCategoryOrderBy.PriceLowToHigh:
                query = query.OrderBy(g =>
                    g.InitialPrice - (g.InitialPrice * g.Discount / 100m));
                break;

            case GamesByCategoryOrderBy.PriceHighToLow:
                query = query.OrderByDescending(g =>
                    g.InitialPrice - (g.InitialPrice * g.Discount / 100m));
                break;

            case GamesByCategoryOrderBy.Relevance:
                query = query
                    .OrderByDescending(g => g.PurchasedGames.Count())
                    .ThenByDescending(g => g.Reviews.Any()
                        ? g.Reviews.Average(r => r.Review1)
                        : 0);
                break;

            default:
                query = query.OrderByDescending(g => g.AdditionDate);
                break;
        }


        return await query
            .Select(g => new GameStoreDTO
            {
                gameId = g.GameId,
                gameName = g.GameName,
                price = g.InitialPrice,
                discount = g.Discount,

                gameCover = g.GamesVidsAndPictures
                    .Where(c => c.IsPrimary && !c.Type)
                    .Select(c => c.Path)
                    .FirstOrDefault(),

                companyName = g.Company.CompanyName,
                companyCover = g.Company.LogoPath,
                gameStatus = g.Status.StatusName,

                additionDate = g.AdditionDate,

                categories = g.GameGenres
                    .Select(gg => gg.Genre.TypeName)
                    .ToList(),

                AverageRating = g.Reviews.Any()
                    ? g.Reviews.Average(r => (decimal)r.Review1)
                    : 0,

                ReviewCount = g.Reviews.Count(),

                TotalSold = g.PurchasedGames.Count()
            })
            .Skip((filter.pageNumber - 1) * filter.pageSize)
            .Take(filter.pageSize)
            .ToListAsync();
    }


    public async Task<FiltrationDataOfGamesBySection> GetGamesBySectionFiltrations(UserStoreSectionType sectionType, int? UserId)
    {
        using var context = CreateDbContext();
        IQueryable<Game> gamesInSection = context.Games;

        var ownedGameIds = context.PurchasedGames
            .Where(pg => pg.UserId == UserId)
            .Select(pg => pg.GameId);

        var purchasedCategories = await context.PurchasedGames
            .Where(pg => pg.UserId == UserId)
            .GroupBy(pg => pg.Game.TypeId)
            .Select(g => new
            {
                CategoryId = g.Key,
                Score = g.Count() * 2 // Give purchases more weight
            })
            .ToListAsync();

        var wishlistCategories = await context.WishLists
            .Where(w => w.UserId == UserId)
            .GroupBy(w => w.Game.TypeId)
            .Select(g => new
            {
                CategoryId = g.Key,
                Score = g.Count()
            })
            .ToListAsync();

        var recommendedCategories = purchasedCategories
            .Concat(wishlistCategories)
            .GroupBy(x => x.CategoryId)
            .Select(g => new
            {
                CategoryId = g.Key,
                Score = g.Sum(x => x.Score)
            })
            .OrderByDescending(x => x.Score)
            .Select(x => x.CategoryId)
            .ToList();

        var purchasedFeatures = await context.PurchasedGames
            .Where(pg => pg.UserId == UserId)
            .SelectMany(pg => pg.Game.GamesFeatures)
            .GroupBy(gf => gf.FeatureId)
            .Select(g => new
            {
                FeatureId = g.Key,
                Score = g.Count() * 2
            })
            .ToListAsync();

        var wishlistFeatures = await context.WishLists
            .Where(w => w.UserId == UserId)
            .SelectMany(w => w.Game.GamesFeatures)
            .GroupBy(gf => gf.FeatureId)
            .Select(g => new
            {
                FeatureId = g.Key,
                Score = g.Count()
            })
            .ToListAsync();

        var recommendedFeatures = purchasedFeatures
            .Concat(wishlistFeatures)
            .GroupBy(x => x.FeatureId)
            .Select(g => new
            {
                FeatureId = g.Key,
                Score = g.Sum(x => x.Score)
            })
            .OrderByDescending(x => x.Score)
            .Select(x => x.FeatureId)
            .ToList();

        switch (sectionType)
        {
            case UserStoreSectionType.HiddenGems:
                // later we will change 0 to 10
                gamesInSection = gamesInSection.Where(g => !ownedGameIds.Contains(g.GameId))
                    .Where(g => g.Reviews.Count() >= 0 &&
                                g.Reviews.Average(r => r.Review1) >= 4.5)
                    .OrderBy(g => g.PurchasedGames.Count())
                    .ThenByDescending(g => g.Reviews.Average(r => r.Review1));
                break;

            case UserStoreSectionType.RecentlyUpdatedGames:
                gamesInSection = gamesInSection.OrderByDescending(g => g.LastUpdate);
                break;

            case UserStoreSectionType.RecommendedForYou:
                gamesInSection = gamesInSection
                    .Where(g =>
                        !ownedGameIds.Contains(g.GameId) &&
                        g.StatusId == 1)
                    .OrderByDescending(g =>
                        (recommendedCategories.Contains(g.TypeId) ? 10 : 0) +
                        (g.GamesFeatures.Count(gf => recommendedFeatures.Contains(gf.FeatureId)) * 3))
                    .ThenByDescending(g => g.PurchasedGames.Count());
                break;

            case UserStoreSectionType.DealsForYou:
                gamesInSection = gamesInSection
                    .Where(g =>
                        !ownedGameIds.Contains(g.GameId) &&
                        g.StatusId == 1 && g.Discount >= 40)
                    .OrderByDescending(g =>
                        (recommendedCategories.Contains(g.TypeId) ? 10 : 0) +
                        (g.GamesFeatures.Count(gf => recommendedFeatures.Contains(gf.FeatureId)) * 3))
                    .ThenByDescending(g => g.Discount);
                break;
        }

        // IMPORTANT: declared AFTER the switch, so it reflects the filtered section, not all games
        var gameIdsInSection = gamesInSection.Select(g => g.GameId);

        var data = new FiltrationDataOfGamesBySection
        {

            maxPrice = await gamesInSection
                .MaxAsync(g => g.InitialPrice - (g.InitialPrice * g.Discount / 100m)),

            minPrice = await gamesInSection
                .MinAsync(g => g.InitialPrice - (g.InitialPrice * g.Discount / 100m)),

            companies = await context.Companies
                .Where(c => c.Games.Any(g => gameIdsInSection.Contains(g.GameId)))
                .Select(c => new CompaniesDTO
                {
                    companyId = c.CompanyId,
                    companyName = c.CompanyName,
                    companyCover = c.LogoPath,
                    gamesCount = c.Games.Count()
                })
                .ToListAsync(),
            categories = await context.GamesTypes
                .Where(c => c.GameGenres.Any(g => gameIdsInSection.Contains(g.GameId)))
                .Select(c => new CategoriesDTO
                {
                    CategoryId = c.Id,
                    CategoryName = c.TypeName,
                    GamesCount = c.GameGenres.Select(g => g.GameId).Distinct().Count()
                })
                .ToListAsync(),

            devices = await context.Devices
                .Where(d => d.GameDevices.Any(gd => gameIdsInSection.Contains(gd.GameId)))
                .Select(d => new DevicesDTO
                {
                    deviceId = d.DeviceId,
                    deviceName = d.DeviceName,
                    deviceCover = d.IconPath
                }).ToListAsync(),

            languages = await context.Languages
                .Where(l => l.GameLanguages.Any(gl => gameIdsInSection.Contains(gl.GameId)))
                .Select(l => new LanguageDTO
                {
                    languageId = l.LanguageId,
                    languageName = l.LanguageName,
                    languageCode = l.LanguageCode
                }).ToListAsync(),

            features = await context.Features
                .Where(f => f.GamesFeatures.Any(gf => gameIdsInSection.Contains(gf.GameId)))
                .Select(f => new FeatureDTO
                {
                    featureId = f.Id,
                    featureName = f.FeatureName
                }).ToListAsync()
        };

        return data;
    }

    public async Task<string?> GetGameNameAsync(int gameId)
    {
        using var context = CreateDbContext();
        return await context.Games.Where
            (g => g.GameId == gameId).Select(g => g.GameName).FirstOrDefaultAsync();
    }
    private async Task<T> RunWithContext<T>(Func<GameMarketplaceContext, Task<T>> query)
    {
        using var ctx = CreateDbContext();
        return await query(ctx);
    }
}


