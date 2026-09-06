using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }

    public DateTime CommentDate { get; set; }

    public string CommentText { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
