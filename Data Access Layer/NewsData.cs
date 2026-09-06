using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.DTO.Auth;

namespace Data_Access_Layer;

public class NewsData : BaseData
{
    public NewsData(IConfiguration config) : base(config) { }
    
    
    public async Task<List<PublicNewsDTO>?> GetPublicNewsAsync(int page , int items)
    {
        using var context = CreateDbContext();
        var data = await context.Games
    .AsNoTracking()
    .OrderByDescending(g => g.LastUpdate)
    .Skip(items * (page - 1))
    .Take(items)
    .Select(g => new PublicNewsDTO
    {
        GameId = g.GameId,
        GameName = g.GameName,
        GameLogo = g.GamesVidsAndPictures.Where(c=> c.GameId == g.GameId && c.IsPrimary==true && c.Type==false) .Select(c=> c.Path).FirstOrDefault(),

        NewsType =
            g.LastUpdate == g.AdditionDate && g.StatusId == 1
                ? "Added"
                : g.LastUpdate > g.AdditionDate && g.StatusId == 1
                    ? "Updated"
                    : null,

        NewsDate = g.LastUpdate,
        GameVersion = g.GameVersion,

        StatusName = g.Status.StatusName,

        CompanyId = g.Company.CompanyId,
        CompanyName = g.Company.CompanyName,
        LogoPath = g.Company.LogoPath
    })
    .ToListAsync();
        return data;
    }
}
