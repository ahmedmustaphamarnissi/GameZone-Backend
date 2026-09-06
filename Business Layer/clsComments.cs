using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer;
using Microsoft.Extensions.Configuration;
using Models.DTO;

namespace Business_Layer;

public class clsComments : BaseService
{
    public clsComments(IConfiguration config) : base(config) { }


    public async Task<CommentDTO?> AddCommentAsync(CommentPostDTO Comment)
    {
        var data = await new CommentsData(_config).AddCommentAsync(Comment);
        if (data == null)
            throw new ArgumentException("Comment is not posted");
        return data;
    }

    public async Task<GetCommentsData?> GetCommentsAsync(CommentsRequest comment, int? userId)
    {
        var data = await new CommentsData(_config).GetCommentsAsync(comment, userId);
        if (data == null)
            throw new ArgumentException("Comments are not found");
        return data;
    }

    public async Task<int?> GetUserIdByCommentIdAsync(int CommentId)
    {
        var data = await new CommentsData(_config).GetUserIdByCommentIdAsync(CommentId);
        return data;
    }

    public async Task<bool> DeleteCommentAsync(int commentId)
    {
        var data = await new CommentsData(_config).DeleteCommentAsync(commentId);
        return data;
    }

    public async Task<CommentDTO?> UpdateCommentAsync(CommentPutDTO newComment)
    {
        var data = await new CommentsData(_config).UpdateCommentAsync(newComment);
        if (data == null)
            throw new ArgumentException("comment can't be updated");
        return data;
    }
}
