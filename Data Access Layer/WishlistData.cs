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
using Models.DTO.Filter;
using static System.Net.WebRequestMethods;

namespace Data_Access_Layer
{
    public class WishlistData : BaseData
    {
        public WishlistData(IConfiguration config) : base(config)
        {

        }

        public async Task<bool> IsGameInWishList(int GameId, int UserId)
        {
            using var context = CreateDbContext();
            return await context.WishLists.AnyAsync(g => g.UserId==UserId && g.GameId ==GameId);
        }

        public async Task<bool> AddGameToWishList(int GameId, int UserId)
        {
            var wishListElement = new WishList
            {
                GameId = GameId,
                UserId = UserId,
                Date = DateTime.Now,
            };

            var context = CreateDbContext();
            context.WishLists.Add(wishListElement);
            return await context.SaveChangesAsync() > 0 ;
        }

        public async Task<bool> RemoveGameFromWishList(int GameId, int UserId)
        {
            var context = CreateDbContext();
            var wishListElement = await context.WishLists.FirstOrDefaultAsync(g => g.UserId == UserId && g.GameId == GameId);
            if (wishListElement == null)
                return false;

            context.WishLists.Remove(wishListElement);
            return await context.SaveChangesAsync() > 0 ;
        }

        public async Task<List<generalGameDTO>?> GetWishListGamesAsync(WishListRequest filter , int ? UserId)
        {
            using var context = CreateDbContext();

            filter.pageNumber = Math.Max(1, filter.pageNumber);
            filter.pageSize = Math.Clamp(filter.pageSize, 1, 50);

            var query = context.Games
                .AsNoTracking()
                .Where(g => g.WishLists.Any(w => w.UserId == UserId));




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
                    companyId = g.CompanyId
                })
                .Skip((filter.pageNumber - 1) * filter.pageSize)
                .Take(filter.pageSize)
                .ToListAsync();
        }

        public async Task<WishListGamesDTO?> GetWishListGamesBySearchAsync(WishListBySearchRequest request, int? UserId)
        {
            using var context = CreateDbContext();


            var query = context.Games
                .AsNoTracking()
                .Where(g => g.WishLists.Any(w => w.UserId == UserId) && g.GameName.Contains(request.searchText));



            var totalGamesCount = await query.CountAsync();
            var games = await query.AsNoTracking()
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
                    companyId = g.CompanyId
                })
                .Skip((request.pageNumber - 1) * request.pageSize)
                .Take(request.pageSize)
                .ToListAsync();
            return new WishListGamesDTO
            {
                games = games,
                countItems = totalGamesCount
            };
        }

        public async Task<FiltrationDataOfGamesBySection> GetWishListGamesFiltrations(int? UserId)
        {
            using var context = CreateDbContext();
            IQueryable<Game> gamesInSection = context.Games.Where(g=> g.WishLists.Any(w=> w.UserId ==UserId));


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
    }
}
