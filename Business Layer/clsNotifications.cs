using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Models.DTO;
using static Business_Layer.clsNotifications;

namespace Business_Layer;

public class clsNotifications : BaseService
{
   
    public clsNotifications(IConfiguration config) : base(config)
    {
    }

    public async Task<NavbarStateDTO?> GetNavbarStateAsync(int userId)
    {
        return await new Data_Access_Layer.NotificationsData(_config).GetNavbarStateAsync(userId);
    }

    public async Task<bool?> ReadUserNotificationsAsync(int userId)
    {
        return await new Data_Access_Layer.NotificationsData(_config).ReadUserNotificationsAsync(userId);
    }
    public async Task<NotificationsResponseDTO?> GetUserNotificationsAsync
        (int userId, int pageNumber, int pageSize)
    {
        return await new Data_Access_Layer.NotificationsData(_config).
            GetUserNotificationsAsync(userId, pageNumber, pageSize);
    }
}
