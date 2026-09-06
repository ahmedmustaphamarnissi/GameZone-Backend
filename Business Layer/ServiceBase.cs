using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Business_Layer;

public class BaseService
{
    protected readonly IConfiguration _config;

    public BaseService(IConfiguration config)
    {
        _config = config;
    }
}