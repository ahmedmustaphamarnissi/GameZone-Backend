using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Business_Layer
{
    public class clsCountry : BaseService
    {
        

        public clsCountry(IConfiguration config) : base(config)
        {
            
        }

        public async Task<int?> GetCountryIdByCountryCodeAsync(string countryCode)
        {
            var Id = await new Data_Access_Layer.CountriesData(_config).GetCountryIdByCountryCodeAsync(countryCode);
            if(Id == null) 
                throw new ArgumentException("Country code not found.");
            return Id;
        }
    }
}
