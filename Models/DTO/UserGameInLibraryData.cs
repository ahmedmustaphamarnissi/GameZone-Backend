using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class UserGameInLibraryData
{
    public int ?installationId { get; set; }
    public DateTime? purchaseDate { get; set; }
    public DateTime? installationDate { get; set; }
    public decimal? purchasePrice { get; set; }
    public bool isInstalled { get; set; }
    public bool hasUpdate { get; set; }
    public bool isPaused { get; set; }
    public double? downloadedBytes { get; set; }
    public string? installationVersion { get; set; } = null!;
    public bool isFavorite { get; set; }
}
