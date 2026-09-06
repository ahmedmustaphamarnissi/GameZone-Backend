using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer;
using Microsoft.Extensions.Configuration;
using Models.DTO;

namespace Business_Layer;

public class clsLanguage : BaseService
{
    public clsLanguage(IConfiguration config) : base(config) { }


    public async Task<List<LanguageDTO>?> GetAllLanguagesAsync()
    {
        var data = await new languageData(_config).GetAllLanguagesAsync();
        if (data == null)
            throw new ArgumentException("Languages are not found");
        return data;
    }
}
