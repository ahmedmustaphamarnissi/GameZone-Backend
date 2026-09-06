using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class PostInstalationGameDTO
{
    public int gameId { get; set; }
    public string installationVersion { get; set; } = null!;
    public DateTime? createdAt { get; set; }
}
