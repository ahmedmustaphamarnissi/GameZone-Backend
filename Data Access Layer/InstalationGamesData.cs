using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameZoneBack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.DTO;
using Models.Data.enums;

namespace Data_Access_Layer;

public class InstalationGamesData : BaseData
{
    public InstalationGamesData(IConfiguration config) : base(config)
    {

    }

    public async Task<bool> CheckIfInstallationGameExist(int InstallationId , int userId)
    {
        using var context = CreateDbContext();
        return await context.InstalledGames.AnyAsync(g => g.Id == InstallationId && g.UserId == userId);
    }

    public async Task<int?> CreateNewInstalationColumn(
        PostInstalationGameDTO postInstalationGameDTO , int userId)
    {
        using var context = CreateDbContext();
        var column = new InstalledGame
        {
            UserId = userId,
            GameId = postInstalationGameDTO.gameId,
            InstallationVersion = postInstalationGameDTO.installationVersion,
            CreatedAt = postInstalationGameDTO.createdAt ?? DateTime.UtcNow,
            DownloadedBytes = 1,
            IsPaused = false,
            IsInstalled = false,
            UpdatedAt = DateTime.UtcNow
        };
        context.InstalledGames.Add(column);
        await context.SaveChangesAsync();
        return column.Id;
    }

    public async Task<bool> UpdateInstallationGame(UpdateInstalationGameDTO request)
    {
        using var context = CreateDbContext();

        var installationGame = await context.InstalledGames
            .FirstOrDefaultAsync(i => i.Id == request.InstallationId);

        if (installationGame == null)
            return false;

        installationGame.UpdatedAt = request.UpdatedAt;

        switch (request.Event)
        {
            case InstalationGamesEvents.pause:

                if (request.DownloadedBytes == null)
                    return false;

                installationGame.IsPaused = true;
                installationGame.DownloadedBytes = request.DownloadedBytes;

                break;

            case InstalationGamesEvents.resume:

                installationGame.IsPaused = false;

                break;

            case InstalationGamesEvents.finishUpdate:

                if (request.DownloadedBytes == null ||
                    string.IsNullOrWhiteSpace(request.InstallationVersion))
                    return false;

                installationGame.IsPaused = false;
                installationGame.IsInstalled = true;
                installationGame.DownloadedBytes = request.DownloadedBytes;
                installationGame.InstallationVersion = request.InstallationVersion;
                installationGame.InstallationDate = request.UpdatedAt;

                break;

            case InstalationGamesEvents.finishInstalation:

                if (request.DownloadedBytes == null ||
                    string.IsNullOrWhiteSpace(request.InstallationVersion))
                    return false;

                installationGame.IsPaused = false;
                installationGame.IsInstalled = true;
                installationGame.DownloadedBytes = request.DownloadedBytes;
                installationGame.InstallationVersion = request.InstallationVersion;
                installationGame.InstallationDate = request.UpdatedAt;

                break;

            default:
                return false;
        }

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteInstallationGame(int InstallationId)
    {
        using var context = CreateDbContext();
        var installationGame = await context.InstalledGames.FindAsync(InstallationId);

        if (installationGame == null)
            return false;

        context.InstalledGames.Remove(installationGame);
        return await context.SaveChangesAsync() > 0;
    }
}
