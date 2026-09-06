using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Data.enums; 

public enum PublicStorePriceFiltration
{
    All = 0,
    Free = 1,
    Paid = 2,
    Under20 = 3,
    Btween20And40 = 4,
    Btween40And60 = 5,
    Over60 = 6,
}
