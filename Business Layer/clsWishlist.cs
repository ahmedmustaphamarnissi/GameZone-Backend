using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameZoneBack.Models;
using Microsoft.Extensions.Configuration;
using Models.DTO;
using Models.DTO.Filter;

namespace Business_Layer
{
    public class clsWishlist : BaseService
    {
        public clsWishlist(IConfiguration config) : base(config) { }


        public async Task<bool> IsGameInWishList(int GameId, int UserId)
        {
            var Data = new Data_Access_Layer.WishlistData(_config);
            return await Data.IsGameInWishList(GameId, UserId);
        }

        public async Task<bool> AddGameToWishList(int GameId, int UserId)
        {
            var Data = new Data_Access_Layer.WishlistData(_config);
            return await Data.AddGameToWishList(GameId, UserId);
        }

        

        public async Task<bool> RemoveGameFromWishList(int GameId, int UserId)
        {
            var Data = new Data_Access_Layer.WishlistData(_config);
            return await Data.RemoveGameFromWishList(GameId, UserId);
        }
        public async Task<FiltrationDataOfGamesBySection> GetWishListGamesFiltrations(int? UserId)
        {
            var Data = await new Data_Access_Layer.WishlistData(_config).GetWishListGamesFiltrations(UserId);
            if(Data==null)
                throw new ArgumentException("Wish list filtrations not found");

            return Data;
        }
        public async Task<List<generalGameDTO>?> GetWishListGamesAsync(WishListRequest filter, int? UserId)
        {
            var Data = await new Data_Access_Layer.WishlistData(_config).GetWishListGamesAsync(filter, UserId);
            if (Data == null)
                throw new ArgumentException("Wishlist is not found");

            return Data;
        }

        public async Task<WishListGamesDTO?> GetWishListGamesBySearchAsync(WishListBySearchRequest request, int? UserId)
        {
            var Data = await new Data_Access_Layer.WishlistData(_config).GetWishListGamesBySearchAsync(request, UserId);
            if (Data == null)
                throw new ArgumentException("Wishlist games with this search text are not found");

            return Data;
        }
    }
}
