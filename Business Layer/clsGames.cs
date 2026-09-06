using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Data_Access_Layer;
using Microsoft.Extensions.Configuration;
using Models.Data.enums;
using Models.DTO;
using Models.DTO.Auth;
using Models.DTO.Filter;
using static System.Net.WebRequestMethods;

namespace Business_Layer;

public class clsGames : BaseService
{
    public clsGames(IConfiguration config) : base(config)
    {

    }

    public async Task<List<GameDTO>?> GetGamesNewReleasesAsync(int pageNumber, int pageSize)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        List<GameDTO>? games = await Data.getNewReleasesAsync(pageNumber, pageSize);
        if (games == null)
            throw new ArgumentException("new games are not found");
        return games;
    }

    public async Task<List<GameDTO>?> getNewReleasesWithFilterAsync(NewReleasesFilterDTO filter)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        List<GameDTO>? games = await Data.getNewReleasesWithFilterAsync(filter);
        if (games == null)
            throw new ArgumentException("new games are not found");
        return games;
    }

    public async Task<List<GameDTO>?> GetCommingSoonGamesAsync(int pageNumber, int pageSize)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        List<GameDTO>? games = await Data.getCommingSoonGamesAsync(pageNumber, pageSize);
        if (games == null)
            throw new ArgumentException("new games are not found");
        return games;
    }

    public async Task<List<GameDTO>?> getCommingSoonGamesWithFilterAsync(NewReleasesFilterDTO filter)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        List<GameDTO>? games = await Data.getCommingSoonGamesWithFilterAsync(filter);
        if (games == null)
            throw new ArgumentException("new games are not found");
        return games;
    }
    public async Task<List<shortGameDTO>?> getTrendingGamesAsync()
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        List<shortGameDTO>? games = await Data.getTrendingGames();
        if (games == null)
            throw new ArgumentException("new games are not found");
        return games;
    }
    public async Task<List<GameStoreDTO>?> getTopSellersGamesAsync(int pageNumber, int pageSize)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        List<GameStoreDTO>? games = await Data.getTopSellersGamesAsync(pageNumber, pageSize);
        if (games == null)
            throw new ArgumentException("top sellers games are not found");
        return games;
    }

    public async Task<List<GameStoreDTO>?> getTopSellersGamesWithFiltrationAsync(TopSellersFilter filter)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        List<GameStoreDTO>? games = await Data.GetTopSellersGamesWithFiltrationAsync(filter);
        if (games == null)
            throw new ArgumentException("top sellers games are not found");
        return games;
    }
    public async Task<PublicStoreDTO?> GetPublicStoreGamesAsync()
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        PublicStoreDTO result = new PublicStoreDTO();
        result.specialOffers = await Data.GetStoreGamesAsync(Models.Data.enums.StoreCategoryType.specialOffers);
        result.freeGames = await Data.GetStoreGamesAsync(Models.Data.enums.StoreCategoryType.freeGames);
        result.topSellers = await Data.GetStoreGamesAsync(Models.Data.enums.StoreCategoryType.topSellers);
        result.newReleases = await Data.GetStoreGamesAsync(Models.Data.enums.StoreCategoryType.newReleases);
        result.commingSoon = await Data.GetStoreGamesAsync(Models.Data.enums.StoreCategoryType.commingSoon);
        result.filtrations.categories = await new Data_Access_Layer.CategoriesData(_config).GetCategoriesAsync();
        result.filtrations.companies = await new Data_Access_Layer.CompaniesData(_config).getAllCompaniesAsync();
        result.filtrations.devices = await new Data_Access_Layer.DevicesData(_config).getAllDevicesAsync();
        result.filtrations.features = await new Data_Access_Layer.FeatureData(_config).GetAllFeaturesAsync();
        result.filtrations.languages = await new Data_Access_Layer.languageData(_config).GetAllLanguagesAsync();
        return result;
    }

    public async Task<UserStoreDTO?> GetUserStoreGamesAsync(int ? UserId)
    {

        var Data = new Data_Access_Layer.GamesData(_config);
        UserStoreDTO result = new UserStoreDTO();
        result.specialOffers = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.specialOffers , UserId);
        result.freeGames = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.freeGames, UserId);
        result.topSellers = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.topSellers, UserId);
        result.newReleases = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.newReleases, UserId);
        result.commingSoon = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.commingSoon, UserId);
        result.wishList = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.wishList, UserId);
        result.recommendedForYou = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.recomended, UserId);
        result.hiddenGems = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.hidden, UserId);
        result.dealsForYou= await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.deals, UserId);
        result.updatedGames = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.updated, UserId);
        result.eventStore = await Data.GetStoreEvent(UserId??-1);
        result.filtrations.categories = await new Data_Access_Layer.CategoriesData(_config).GetCategoriesAsync();
        result.filtrations.companies = await new Data_Access_Layer.CompaniesData(_config).getAllCompaniesAsync();
        result.filtrations.devices = await new Data_Access_Layer.DevicesData(_config).getAllDevicesAsync();
        result.filtrations.features = await new Data_Access_Layer.FeatureData(_config).GetAllFeaturesAsync();
        result.filtrations.languages = await new Data_Access_Layer.languageData(_config).GetAllLanguagesAsync();
        return result;
    }

    public async Task<UserStoreDTO?> GetUserStoreGamesWithFiltrationAsync(int ? UserId,PublicStoreFiltrationRequest? filtration = null)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        UserStoreDTO result = new UserStoreDTO();
        result.specialOffers = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.specialOffers, UserId,filtration);
        result.freeGames = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.freeGames, UserId, filtration);
        result.topSellers = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.topSellers, UserId, filtration);
        result.newReleases = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.newReleases, UserId, filtration);
        result.commingSoon = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.commingSoon, UserId, filtration);
        result.wishList = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.wishList, UserId, filtration);
        result.recommendedForYou = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.recomended, UserId, filtration);
        result.hiddenGems = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.hidden, UserId, filtration);
        result.dealsForYou = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.deals, UserId, filtration);
        result.updatedGames = await Data.GetUserStoreGamesAsync(Models.Data.enums.StoreCategoryType.updated, UserId, filtration);
        return result;
    }
    public async Task<PublicStoreDTO?> GetPublicStoreGamesWithFiltrationAsync(PublicStoreFiltrationRequest? filtration = null)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        PublicStoreDTO result = new PublicStoreDTO();
        result.specialOffers = await Data.GetStoreGamesAsync(Models.Data.enums.StoreCategoryType.specialOffers, filtration);
        result.freeGames = await Data.GetStoreGamesAsync(Models.Data.enums.StoreCategoryType.freeGames, filtration);
        result.topSellers = await Data.GetStoreGamesAsync(Models.Data.enums.StoreCategoryType.topSellers, filtration);
        result.newReleases = await Data.GetStoreGamesAsync(Models.Data.enums.StoreCategoryType.newReleases, filtration);
        result.commingSoon = await Data.GetStoreGamesAsync(Models.Data.enums.StoreCategoryType.commingSoon, filtration);
        return result;
    }

    public async Task<List<shortGameDTO>?> GetpublicStoreSearchGames(string search, int pageSize )
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        var result = await Data.GetpublicStoreSearchGames(search, pageSize);
        if (result == null)
            throw new ArgumentException("no games avilable");
        return result; 
    }

    public async Task<List<GameDTO>?> getFreeGamesWithFilterAsync(FreeGamesFilterDTO filter)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        List<GameDTO>? games = await Data.getFreeGamesWithFilterAsync(filter);
        if (games == null)
            throw new ArgumentException("free games are not found");
        return games;
    }
    public async Task<List<GameStoreDTO>?> GetSpecialOffersGamesWithFiltrationAsync(SpecialOffersFilter filter)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        List<GameStoreDTO>? games = await Data.GetSpecialOffersGamesWithFiltrationAsync(filter);
        if (games == null)
            throw new ArgumentException("speacial offers games are not found");
        return games;
    }
    public async Task<List<GameStoreDTO>?> GetGamesByCategoryIdWithFiltrationAsync(GamesByCategoriesFiltration filter)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        List<GameStoreDTO>? games = await Data.GetGamesByCategorieIdWithFiltrationAsync(filter);
        if (games == null)
            throw new ArgumentException("games in this category are not found");
        return games;
    }
    public async Task<List<GameStoreDTO>?> GetGamesByPublisherIdWithFiltrationAsync(GamesByPublisherFiltration filter)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        List<GameStoreDTO>? games = await Data.GetGamesByPublisherIdWithFiltrationAsync(filter);
        if (games == null)
            throw new ArgumentException("games created with this publisher are not found");
        return games;
    }
    public async Task<FiltrationDataOfGamesByCategory?> GetGamesByCategoryFiltrations(int Id)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        FiltrationDataOfGamesByCategory ? filtration = await Data.GetGamesByCategoryFiltrations(Id);
        if (filtration == null)
            throw new ArgumentException("filtration of catgory games are not found");
        return filtration;
    }
    public async Task<FiltrationDataOfGamesByPublisher?> GetGamesByPublisherFiltrations(int Id)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        FiltrationDataOfGamesByPublisher? filtration = await Data.GetGamesByPublisherFiltrations(Id);
        if (filtration == null)
            throw new ArgumentException("filtration of publicher games are not found");
        return filtration;
    }

    public async Task<bool> CheckIfGameExist(int Id)
    {
        return await new GamesData(_config).CheckIfGameExist(Id);
    }

    public async Task<GameHoverDTO?> GetHoverOnGameDataAsync(int Id)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        GameHoverDTO? gameHoverData = await Data.GetHoverOnGameDataAsync(Id);
        if (gameHoverData == null)
            throw new ArgumentException("game hover data is not found");
        return gameHoverData;
    }

    public async Task<GameDetailsDTO?> GetGameDetailsAsync(int Id ,int? UserId = null)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        GameDetailsDTO? gameDetails = await Data.GetGameDetailsDataAsync(Id, UserId);
        if (gameDetails == null)
            throw new ArgumentException("game details is not found");
        return gameDetails;
    }

    public async Task<List<GameStoreDTO>?> GetGamesBySectionWithFiltrationAsync(GamesBySectionFiltrations filter , int ? UserId)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        List<GameStoreDTO>? games = await Data.GetGamesBySectionWithFiltrationAsync(filter , UserId);
        if (games == null)
            throw new ArgumentException("games in this section are not found");
        return games;
    }

    public async Task<FiltrationDataOfGamesBySection?> GetGamesBySectionFiltrations(UserStoreSectionType sectionType , int ? UserId)
    {
        var Data = new Data_Access_Layer.GamesData(_config);
        FiltrationDataOfGamesBySection? filtration = await Data.GetGamesBySectionFiltrations(sectionType,UserId);
        if (filtration == null)
            throw new ArgumentException("filtration of games by section are not found");
        return filtration;
    }

}

