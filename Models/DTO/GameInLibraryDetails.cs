using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class GameInLibraryDetails
{
    public int gameId { get; set; }
    public string gameName { get; set; } = null!;
    public string? gameDescription { get; set; }
    public decimal initialPrice { get; set; }
    public decimal rating { get; set; }
    public int reviewsCount { get; set; }
    public DateTime additionDate { get; set; }
    public DateTime updateDate { get; set; }
    public double size { get; set; }
    public int discount { get; set; }
    public string gameVersion { get; set; } = null!;
    public string gameStatus { get; set; } = null!;
    public string? coverPath { get; set; }
    public List<CategoriesDTO>? categories { get; set; }
    public CompaniesDTO? company { get; set; }
    public List<LanguageDTO>? languages { get; set; }
    public List<FeatureDTO>? features { get; set; }
    public UserGameInLibraryData? userData { get; set; }
}
