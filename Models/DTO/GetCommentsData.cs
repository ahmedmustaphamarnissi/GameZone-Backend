using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class GetCommentsData
{
    public List<CommentDTO> ? comments { get; set; } = new List<CommentDTO>();
    public int CommentsFiltrationsCount { get; set; }
}
