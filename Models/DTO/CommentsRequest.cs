using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums.Sorts;

namespace Models.DTO;

public class CommentsRequest
{
    public int commentsCount { get; set; }
    public CommentsOrderBy orderBy { get; set; }
    public int gameId { get; set; }
    public bool MyCommentsOnly { get; set; }
}
