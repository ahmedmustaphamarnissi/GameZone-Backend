using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.DTO;

namespace Data_Access_Layer;

public class languageData : BaseData
{
    public languageData(IConfiguration config) : base(config)
    {

    }

    public async Task<List<LanguageDTO>?> GetAllLanguagesAsync()
    {
        using var context = CreateDbContext();
        var query = context.Languages.AsNoTracking().OrderBy(c => c.LanguageName);


        var data = await query
            .Select(g => new LanguageDTO
            {
                languageId = g.LanguageId,
                languageName = g.LanguageName
            }).ToListAsync();
        return data;
    }
}
