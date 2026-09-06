using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class WishListBySearchRequest
{
    [Required]
    [MinLength(2, ErrorMessage = "Search text must be at least 2 character long.")]
    public string searchText { get; set; } = null!;
    public int pageNumber { get; set; } = 1;
    public int pageSize { get; set; } = 8;
}
