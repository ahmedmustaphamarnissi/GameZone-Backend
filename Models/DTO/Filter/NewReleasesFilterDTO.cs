using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums;
using Models.Data.enums.Sorts;

namespace Models.DTO.Filter;

public class NewReleasesFilterDTO
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;

    public int[]? CategoryIds { get; set; }

    public int[]? featureIds { get; set; }
    public ReleasePeriod ReleasePeriod { get; set; }
    public NewReleaseOrderBy OrderBy { get; set; }

    public PriceFilter PriceFilter { get; set; }
}
