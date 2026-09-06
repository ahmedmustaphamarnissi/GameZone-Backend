using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;  

public class DevicesDTO
{
    public int deviceId { get; set;}
    public string deviceName { get; set;} = null!;
    public string deviceCover { get; set; } = null!;
}
