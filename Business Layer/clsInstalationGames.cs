using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameZoneBack.Models;
using Microsoft.Extensions.Configuration;
using Models.DTO;

namespace Business_Layer;

public class clsInstalationGames : BaseService
{
    public clsInstalationGames(IConfiguration config) : base(config)
    {

    }

    public async Task<int?> CreateNewInstalationColumn(
        PostInstalationGameDTO postInstalationGameDTO, int userId)

    {
        var data = await new Data_Access_Layer.InstalationGamesData(_config).CreateNewInstalationColumn(postInstalationGameDTO, userId);
        if (data == null)
            throw new ArgumentException("Error creating new instalation column");
        return data;

    }

    public async Task<bool> CheckIfInstallationGameExist(int InstallationId, int userId)
    {
        return await new Data_Access_Layer.InstalationGamesData(_config).CheckIfInstallationGameExist(InstallationId, userId);
    }

    public async Task<bool> UpdateInstalationGame(UpdateInstalationGameDTO request)
    {
        return await new Data_Access_Layer.InstalationGamesData(_config).UpdateInstallationGame(request);
    }

    public async Task<bool> DeleteInstallationGame(int InstallationId)
    {
        return await new Data_Access_Layer.InstalationGamesData(_config).DeleteInstallationGame(InstallationId);
    }

    
}