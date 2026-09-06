using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums;

namespace Models.DTO;

public class UpdateInstalationGameDTO
{
    public int InstallationId { get; set; }
    public InstalationGamesEvents Event { get; set; }
    public long DownloadedBytes { get; set; } 
    public string? InstallationVersion { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
