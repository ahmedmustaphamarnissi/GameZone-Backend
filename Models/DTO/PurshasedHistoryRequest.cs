using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums;
using Models.Data.enums.Sorts;

namespace Models.DTO;

public class PurshasedHistoryRequest
{
    public int  pageNumber { get; set; } = 1;
    public int pageSize { get; set; } = 6;
    public PurshasedHistoryFilter ? Filter { get; set; } 
    public LongReleaseDate? ReleaseDate { get; set; }
    public PurshasedHistoryOrderBy? OrderBy { get; set; } 
}
