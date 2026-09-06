using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer;
using Microsoft.Extensions.Configuration;
using Models.DTO;

namespace Business_Layer;

public class clsCategories : BaseService
{
    public clsCategories(IConfiguration config) : base(config) { }

    public async Task<List<CategoriesDTO>?> GetCategoriesAsync()
    {
        var data = await new CategoriesData(_config).GetCategoriesAsync();
        if(data == null)
             throw new ArgumentException("Categories are not found");
        return data;
    }
    public async Task<bool> CheckIfCategorieExist(int Id)
    {
       return  await new CategoriesData(_config).CheckIfCategorieExist(Id);
    }
}
