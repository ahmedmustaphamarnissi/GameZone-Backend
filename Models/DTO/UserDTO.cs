using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class UserDTO
{
    public int PersonId { get; set;}
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int StatusID  { get; set; }
}
