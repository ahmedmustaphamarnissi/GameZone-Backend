using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameZoneBack.Models;
using Microsoft.Extensions.Configuration;
using Models.Data.enums;
using Models.DTO;
using Models.DTO.Auth;


namespace Business_Layer
{
    public class clsUser : BaseService
    {
        public clsUser(IConfiguration config) : base(config) { }

        public async Task<User?> GetUserByUserNameAsync(string username)
        {
            var data = new Data_Access_Layer.UserData(_config);
            var user = await data.GetUserByUserNameAsync(username);
            if (user == null)
                throw new ArgumentException("user is not found");
            return user;
        }
        public async Task<User?> AddNewUserAsync(RegisterRequest request, int CountryId)
        {

            PersonDTO person = new PersonDTO
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                CountryId = CountryId,
                DateOfBirth = request.DateOfBirth,
                Email = request.Email,
            };

            UserDTO user = new UserDTO
            {
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                UserName = request.UserName,
                StatusID = (int)PersonStatus.Active,
            };
            var data = new Data_Access_Layer.UserData(_config);
            var result = await data.AddNewUserAsync(person, user);
            if (result == null)
                throw new ArgumentException("user is not added");
            return result;
        }

        public async Task<int?> GetUserIdByEmailAsync(string UserName)
        {
            var data = new Data_Access_Layer.UserData(_config);

            return await data.GetUserIdByEmailAsync(UserName);
        }

        public async Task<User?> GetUserByUserIdAsync(int userId)
        {
            var data = new Data_Access_Layer.UserData(_config);

            return await data.GetUserByUserIdAsync(userId);
        }
        public async Task<bool> CheckIfUserExist(int Id)
        {
            var data = new Data_Access_Layer.UserData(_config);
            return await data.CheckIfUserExistAsync(Id);
        }
        public async Task<bool> CheckIfUserProfileExistAsync(int Id)
        {
            var data = new Data_Access_Layer.UserData(_config);
            return await data.CheckIfUserProfileExistAsync(Id);
        }
        public async Task<bool> CheckIfTheUserBlocked(int Id, int currentUserId)
        {
            var data = new Data_Access_Layer.UserData(_config);
            return await data.CheckIfTheUserBlocked(Id, currentUserId);
        }
        public async Task<ProfileDTO?> GetUserProfileAsync(
    int Id,
    bool IsMyProfile = false,
    int? myId = null)
        {
            var data = await new Data_Access_Layer.UserData(_config).GetUserProfile(Id, IsMyProfile, myId);
            if (data == null)
                throw new ArgumentException("not found user profile");
            return data;
        }

        public async Task<UserDashboardDTO?> GetUserDashboardAsync(int userId)
        {
            var data = await new Data_Access_Layer.UserData(_config).GetUserDashboardAsync(userId);
            if (data == null)
                throw new ArgumentException("not found user dashboard");
            return data;
        }
        public async Task<UserProfileSettingsDTO?> GetUserProfileSettingsAsync(int userId)
        {
            var data = await new Data_Access_Layer.UserData(_config).GetUserProfileSettingsAsync(userId);
            if (data == null)
                throw new ArgumentException("not found user dashboard");
            return data;
        }

        public async Task<bool?> InactiveUserAccount(int userId)
        {
            return await new Data_Access_Layer.UserData(_config).InactiveUserAccount(userId);
        }

        public async Task<bool?> ChangeUserPassword(int userId, string Password)
        {
            return await new Data_Access_Layer.UserData(_config).ChangeUserPassword(userId,Password);
        }

        public async Task<bool?> ChangeUserCredentialsAsync(
    int userId,
    UpdateUserDTO request)
        {
            return await new Data_Access_Layer.UserData(_config).
                ChangeUserCredentialsAsync(userId, request);
        }
    }
}
