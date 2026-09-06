using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class UserGameData
{
    public bool ownsGame { get; set; }
    public bool isInWishlist { get; set; }
    public ReviewDTO? userReview { get; set; }
    public int[]? userCommentsIds { get; set; }
}
