using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums;

namespace Models.DTO.Filter;

public class SpecialOffersFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;

    public int[]? CategoryIds { get; set; }

    public SpecialOffersOrderBy ? OrderBy { get; set; }
}
