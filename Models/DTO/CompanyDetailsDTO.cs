using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class CompanyDetailsDTO
{
    public int companyId { get; set; }

    public string companyName { get; set; } = null!;

    public string? logoPath { get; set; }

    public string? website { get; set; }

    public string? description { get; set; }

    public DateTime? additionDate { get; set; }
    public string? countryName { get; set; } 
    public string? countryCode { get; set; }
    public int totalGames { get; set; }
    public double averageRating { get; set; }
    public int totalReviews { get; set; }
    public List<shortGameDTO>? games { get; set; } = [];
    public List<string>? popularGenres { get; set; } = [];
}
