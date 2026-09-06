using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Data.enums.Sorts;
using Models.DTO;

namespace Data_Access_Layer;

public class CompaniesData : BaseData
{
    public CompaniesData(IConfiguration config) : base(config)
    {

    }

    public async Task<List<CompaniesDTO>?> getCompaniesAsync(int PageNumber , int PageSize, PublishersOrderBy order)
    {
        using var context = CreateDbContext();
        var query = context.Companies.AsNoTracking().Skip((PageNumber - 1) * PageSize)
            .Take(PageSize);

        switch (order)
        {
            case PublishersOrderBy.NameAZ:
                query = query.OrderBy(c => c.CompanyName);
                break;
            case PublishersOrderBy.Newest:
                query = query.OrderByDescending(c => c.CreatedAt);
                break;
            case PublishersOrderBy.Oldest:
                query = query.OrderBy(c => c.CreatedAt);
                break;
            case PublishersOrderBy.MostGames:
                query = query.OrderByDescending(c => c.Games.Count());
                break;
            default:
                query = query.OrderByDescending(c => c.Games.Count());
                break;
        }

        var data = await query
            .Select(g => new CompaniesDTO
            {
                companyId = g.CompanyId,
                companyName = g.CompanyName,
                countryName = g.Country.CountryName,
                countryCode = g.Country.CountryCode,
                companyCover = g.LogoPath,
                gamesCount = g.Games.Count()
            }).ToListAsync();
        return data;
    }


    public async Task<List<CompaniesDTO>?> getAllCompaniesAsync()
    {
        using var context = CreateDbContext();
        var query = context.Companies.AsNoTracking().OrderByDescending(c => c.Games.Count());


        var data = await query
            .Select(g => new CompaniesDTO
            {
                companyId = g.CompanyId,
                companyName = g.CompanyName,
                countryName = g.Country.CountryName,
                countryCode = g.Country.CountryCode,
                companyCover = g.LogoPath,
                gamesCount = g.Games.Count()
            }).ToListAsync();
        return data;
    }

    public async Task<List<CompaniesDTO>?> getCompaniesBySearchAsync(int PageSize , string search)
    {
        using var context = CreateDbContext();
        var data = await context.Companies.AsNoTracking().Where(g => g.CompanyName.Contains(search)).Skip(0)
            .Take(PageSize).OrderByDescending(g => g.Games.Count())
            .Select(g => new CompaniesDTO
            {
                companyId = g.CompanyId,
                companyName = g.CompanyName,
                countryName = g.Country.CountryName,
                countryCode = g.Country.CountryCode,
                companyCover = g.LogoPath,
                gamesCount = g.Games.Count()
            }).ToListAsync();
        return data;
    }

    public async Task<bool> CheckIfCompanyExist(int Id)
    {
        using var context = CreateDbContext();
        return await context.Companies.AnyAsync(g => g.CompanyId == Id);
    }


    public async Task<CompanyDetailsDTO?> GetCompanyDetailsAsync(int id)
    {
        using var context = CreateDbContext();

        return await context.Companies
            .AsNoTracking()
            .Where(c => c.CompanyId == id)
            .Select(c => new CompanyDetailsDTO
            {
                // Basic Information
                companyId = c.CompanyId,
                companyName = c.CompanyName,
                logoPath = c.LogoPath,
                website = c.Website,
                description = c.Description,
                additionDate = c.CreatedAt,

                // Country
                countryName = c.Country.CountryName,
                countryCode = c.Country.CountryCode,

                // Statistics
                totalGames = c.Games.Count(),

                averageRating = c.Games
                    .SelectMany(g => g.Reviews)
                    .Average(r => (double?)r.Review1) ?? 0,
                totalReviews = context.Games.Where(g=> g.CompanyId == id).Select(g=> g.Reviews.Count()).FirstOrDefault(),
                // Featured Games
                games = c.Games
                    .OrderByDescending(g => g.PurchasedGames.Count())
                    .Take(3)
                    .Select(g => new shortGameDTO
                    {
                        gameId = g.GameId,
                        gameName = g.GameName,
                        gameCover = g.GamesVidsAndPictures
                            .Where(m => m.IsPrimary && !m.Type)
                            .Select(m => m.Path)
                            .FirstOrDefault(),

                        price = g.InitialPrice,
                        discount = g.Discount,

                        categories = g.GameGenres
                            .Select(gg => gg.Genre.TypeName)
                            .ToList()
                    })
                    .ToList(),

                // Popular Genres
                popularGenres = c.Games
                    .SelectMany(g => g.GameGenres)
                    .GroupBy(gg => new
                    {
                        gg.Genre.Id,
                        gg.Genre.TypeName
                    })
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key.TypeName)
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }
}
