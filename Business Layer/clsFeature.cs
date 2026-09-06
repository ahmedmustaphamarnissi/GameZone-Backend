using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer;
using Microsoft.Extensions.Configuration;
using Models.DTO;

namespace Business_Layer;

public class clsFeature : BaseService
{
    public clsFeature(IConfiguration config) : base(config) { }

    public async Task<List<FeatureDTO>?> GetAllFeaturesAsync()
    {
        var data = await new FeatureData(_config).GetAllFeaturesAsync();
        if (data == null)
            throw new ArgumentException("Features are not found");
        return data;
    }
}
