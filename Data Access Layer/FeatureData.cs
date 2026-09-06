using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.DTO;

namespace Data_Access_Layer;

public class FeatureData : BaseData
{
    public FeatureData(IConfiguration config) : base(config)
    {

    }

    public async Task<List<FeatureDTO>?> GetAllFeaturesAsync()
    {
        using var context = CreateDbContext();
        var query = context.Features.AsNoTracking().OrderBy(c => c.FeatureName);


        var data = await query
            .Select(g => new FeatureDTO
            {
                featureId = g.Id,
                featureName = g.FeatureName
            }).ToListAsync();
        return data;
    }
}
