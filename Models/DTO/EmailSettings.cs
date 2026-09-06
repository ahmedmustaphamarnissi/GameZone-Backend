using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class EmailSettings
    {
        public string SenderEmail { get; set; } = null!;
        public string SenderName { get; set; } = null!;
        public string Password { get; set; } = null!;

    }
}
