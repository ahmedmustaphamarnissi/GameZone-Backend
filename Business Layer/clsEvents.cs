using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Models.DTO;

namespace Business_Layer;

public class clsEvents : BaseService
{
    public clsEvents(IConfiguration config) : base(config) { }

    public async Task<EventDTO> GetEventDetails()
    {
        var data =  await new Data_Access_Layer.EventsData(_config).GetEventDetailsAsync();
        if (data == null)
            throw new ArgumentException("Not found any event active now");
        return data;
    }
}
