using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class LibraryGameItemDTO
{
    public int gameId {  get; set; }
    public string gameName { get; set; } = null!;
    public string? coverPicture { get; set; }
    public bool isInstalled { get; set; }
    public bool hasUpdate { get; set; }
    public bool isPaused { get; set; }
    public bool isFavorite { get; set; }

}
