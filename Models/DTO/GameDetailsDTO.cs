using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class GameDetailsDTO
{
    public int gameId { get; set;}
    public string gameName { get; set; } = null!;
    public string? gameDescription { get; set; }
    public decimal initialPrice { get; set; }
    public decimal rating { get; set; }
    public int reviewsCount { get; set; }
    public int commentsCount { get; set; }
    public DateTime additionDate { get; set; }
    public DateTime updateDate { get; set; }
    public int appropriateAge { get; set; }
    public double size { get; set; }
    public int discount { get; set; }
    public string gameVersion { get; set; } = null!;
    public string gameStatus { get; set; } = null!;
    public string? trailerLink { get; set; }
    public string ? coverPath { get; set; }
    public List<string>? screenshotsPaths { get; set; }
    public List<DevicesDTO> devices { get; set; } = new List<DevicesDTO>();
    public List<CategoriesDTO>? categories { get; set; }
    public CompaniesDTO? company { get; set; }
    public ReviewsDashboardDTO? reviewsDashboard { get; set; }
    public List<ReviewDTO>? reviews { get; set; }
    public List<CommentDTO>? comments { get; set; }
    public List<shortGameDTO>? similarGames { get; set; }
    public List<shortGameDTO>?companyGames { get; set; }
    public List<LanguageDTO>? languages { get; set; }
    public List<FeatureDTO>? features { get; set; }
    public SystemRequirementDTO? minimumSystemRequirements { get; set; }
    public SystemRequirementDTO? recommendedSystemRequirements { get; set; }
    public UserGameData? userData { get; set; }

}
