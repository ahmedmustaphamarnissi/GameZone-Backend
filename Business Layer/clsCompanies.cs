using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer;
using Microsoft.Extensions.Configuration;
using Models.Data.enums.Sorts;
using Models.DTO;

namespace Business_Layer;

public class clsCompanies : BaseService
{
    public clsCompanies(IConfiguration config) : base(config)
    {

    }

    public async Task<List<CompaniesDTO>?> getCompaniesAsync(int pageNumber, int pageSize , PublishersOrderBy order)
    {
        var Data = new Data_Access_Layer.CompaniesData(_config);
        List<CompaniesDTO>? companies = await Data.getCompaniesAsync(pageNumber, pageSize,order);
        if (companies == null)
            throw new ArgumentException("companies are not found");
        return companies;
    }

    public async Task<List<CompaniesDTO>?> getCompaniesBySearchAsync(int PageSize, string search)
    {
        var Data = new Data_Access_Layer.CompaniesData(_config);
        List<CompaniesDTO>? companies = await Data.getCompaniesBySearchAsync(PageSize,search);
        if (companies == null)
            throw new ArgumentException("companies are not found");
        return companies;
    }

    public async Task<bool> CheckIfCompanyExist(int Id)
    {
        return await new CompaniesData(_config).CheckIfCompanyExist(Id);
    }

    public async Task<CompanyDetailsDTO?> getCompanyDetailsAsync(int Id)
    {
        var Data = new Data_Access_Layer.CompaniesData(_config);
        CompanyDetailsDTO? company = await Data.GetCompanyDetailsAsync(Id);
        if (company == null)
            throw new ArgumentException("company details are not found");
        return company;
    }
}
