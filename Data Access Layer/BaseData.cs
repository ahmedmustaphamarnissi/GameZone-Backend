using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameZoneBack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data_Access_Layer;

public class BaseData
{
    protected readonly IConfiguration _config;

    public BaseData(IConfiguration config)
    {
        _config = config;
    }

    protected GameMarketplaceContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<GameMarketplaceContext>()
            .UseSqlServer(_config.GetConnectionString("DefaultConnection"))
            .Options;

        return new GameMarketplaceContext(options);
    }
}
