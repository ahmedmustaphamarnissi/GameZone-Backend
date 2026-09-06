using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameZoneBack.Models;
using Microsoft.Extensions.Configuration;
using Models.Data.enums;
using Models.DTO;

namespace Business_Layer;

public class clsPurshasedGames : BaseService
{
    public clsPurshasedGames(IConfiguration config) : base(config) { }

    public async Task<bool> IsGamePurshased(int GameId, int UserId)
    {
        var Data = new Data_Access_Layer.PurshasedGamesData(_config);
        return await Data.IsGamePurshased(GameId, UserId);
    }
    public async Task<List<LibraryGameItemDTO>?> GetPurchasedGames(int? userId)
    {
        var Data = await new Data_Access_Layer.PurshasedGamesData(_config).GetPurchasedGames(userId);
        if (Data == null)
            throw new ArgumentException("Purchased games not found");
        return Data;
    }
    public async Task<GameInLibraryDetails?> GetPurchasedGameDetailsAsync(int gameId, int? userId)
    {
        var Data = await new Data_Access_Layer.PurshasedGamesData(_config).GetPurchasedGameDetails(gameId, userId);
        if (Data == null)
            throw new ArgumentException("Purchased game details not found");
        return Data;
    }
    public async Task<PurshasedHistoryDTO?> GetPurshasedHistoryAsync(PurshasedHistoryRequest request, int? userId)
    {
        var Data = await new Data_Access_Layer.PurshasedGamesData(_config).GetPurshasedHistoryAsync(request, userId);
        if (Data == null)
            throw new ArgumentException("Purchased games history is not found");
        return Data;
    }

    public async Task<bool> PerformActionOnFavoriteGame(int userId, int GameId, bool IsToFavorite)
    {
        return await new Data_Access_Layer.PurshasedGamesData(_config).PerformActionOnFavoriteGame(userId, GameId, IsToFavorite);
    }

    public async Task<bool?> PurchaseAGameAsync(int userId, int GameId, int paymentId, bool isGift = false)
    {
        var res = await new Data_Access_Layer.PurshasedGamesData(_config).
            PurchaseAGameAsync(userId, GameId, paymentId);
        if (res == true)
        {
            string? gameName = await new Data_Access_Layer.GamesData(_config).GetGameNameAsync(GameId); // fetch for the notification body

            if (isGift)
            {
                await new Data_Access_Layer.NotificationsData(_config)
                    .NotificateUserAsync(userId, enNotificationType.GameGift, gameName ?? "game");
            }
            else
            {
                await new Data_Access_Layer.NotificationsData(_config)
                    .NotificateUserAsync(userId, enNotificationType.PurchaseSuccessful, gameName ?? "game");
            }
        }
        return res;
    }
    public async Task<PurchaseCredentials?> GetUserPurchaseCredentials(int userId, int gameId)
    {
        return await new Data_Access_Layer.PurshasedGamesData(_config)
            .GetUserPurchaseCredentials(userId, gameId);
    }
}
