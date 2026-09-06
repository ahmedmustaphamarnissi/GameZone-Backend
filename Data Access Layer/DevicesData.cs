using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.DTO;

namespace Data_Access_Layer;

public class DevicesData : BaseData
{
    public DevicesData(IConfiguration config) : base(config)
    {

    }
    public async Task<List<DevicesDTO>?> getAllDevicesAsync()
    {
        using var context = CreateDbContext();
        var data = await context.Devices.AsNoTracking().OrderBy(d => d.DeviceName).Select(d => new DevicesDTO
        {
            deviceId = d.DeviceId,
            deviceName = d.DeviceName,
            deviceCover = d.IconPath
        }).ToListAsync();
        return data;
    }
}
