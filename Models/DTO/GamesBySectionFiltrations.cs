using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums;
using Models.Data.enums.Sorts;

namespace Models.DTO;

public class GamesBySectionFiltrations
{
    public UserStoreSectionType sectionType { get; set; }
    public decimal? maxPrice { get; set; }
    public decimal? minPrice { get; set; }
    public int pageNumber { get; set; } = 1;
    public int pageSize { get; set; } = 12;
    public int[]? companiesIds { get; set; }
    public int[]? categoryIds { get; set; }
    public int[]? DevicesIds { get; set; }
    public int[]? featureIds { get; set; }
    public int[]? languageIds { get; set; }
    public DiscountChoice? discount { get; set; }
    public LongReleaseDate? releaseDate { get; set; }
    public RatingFilter? rating { get; set; }
    public GamesByCategoryOrderBy? orderBy { get; set; }
}
