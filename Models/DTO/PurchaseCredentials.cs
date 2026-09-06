using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class PurchaseCredentials
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string GameName { get; set; } = null!;
    public decimal PurchasePrice { get; set; }
}
