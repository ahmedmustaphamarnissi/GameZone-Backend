using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer
{
    public class CountriesData : BaseData
    {
        public CountriesData(IConfiguration config) : base(config) 
        {
        }
        public async Task<int?> GetCountryIdByCountryCodeAsync(string countryCode)
        {
            using var context = CreateDbContext();
            var Id = await context.Countries.Where(c => c.CountryCode == countryCode).Select(c => c.CountryId).SingleOrDefaultAsync();
            return Id;
        }
    }
}
