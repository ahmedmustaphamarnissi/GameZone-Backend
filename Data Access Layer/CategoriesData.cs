using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameZoneBack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Models.DTO;

namespace Data_Access_Layer;

public class CategoriesData : BaseData
{
    public CategoriesData(IConfiguration config) : base(config)
    {
        
    }


    public async Task<List<CategoriesDTO> ?> GetCategoriesAsync()
    {
        using var context = CreateDbContext();
        var data = await context.GamesTypes.AsNoTracking().Select(g => new CategoriesDTO
        {
            CategoryId = g.Id,
            CategoryName = g.TypeName,
            GamesCount = g.GameGenres.Count(),
        }).ToListAsync();
        return data; 
    }
    public async Task<bool> CheckIfCategorieExist(int Id)
    {
        using var context = CreateDbContext();
        return await context.GamesTypes.AnyAsync(g => g.Id == Id);
    }
}
