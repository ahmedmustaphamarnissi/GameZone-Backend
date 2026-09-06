using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameZoneBack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Data.enums.Sorts;
using Models.DTO;

namespace Data_Access_Layer;

public class CommentsData : BaseData
{
    public CommentsData(IConfiguration config) : base(config)
    {

    }

    public async Task<CommentDTO?> AddCommentAsync(CommentPostDTO Comment)
    {
        try
        {
            var comment = new Comment
            {
                UserId = Comment.userId,
                GameId = Comment.gameId,
                CommentText = Comment.commentText,
                CommentDate = Comment.commentDate,
            };


            var context = CreateDbContext();
            context.Comments.Add(comment);
            await context.SaveChangesAsync();

            var user = await context.Users
                .Where(u => u.UserId == comment.UserId)
                .Select(u => new { u.UserName, PicturePath = u.Picture != null ? u.Picture.Path : null })
                .FirstOrDefaultAsync();

            return new CommentDTO
            {
                commentId = comment.CommentId,
                userName = user?.UserName,
                userImage = user?.PicturePath,
                userId = comment.UserId,
                gameId = comment.GameId,
                commentText = comment.CommentText,
                commentDate = comment.CommentDate
            };
        }
        catch (Exception ex)
        {
            throw; // preserves original stack trace instead of `throw ex;`
        }
    }


    public async Task<GetCommentsData?> GetCommentsAsync(CommentsRequest comment, int? userId)
    {
        var context = CreateDbContext();

        var query = context.Comments.Where(c => c.GameId == comment.gameId);

        if (comment.MyCommentsOnly)
        {
            query = query.Where(c => c.UserId == userId);
        }

        switch (comment.orderBy)
        {
            case CommentsOrderBy.NewestFirst:
                query = query.OrderByDescending(cc => cc.CommentDate);
                break;
            case CommentsOrderBy.OldestFirst:
                query = query.OrderBy(cc => cc.CommentDate);
                break;
            default:
                query = query.OrderByDescending(cc => cc.CommentDate);
                break;
        }

        var comments = await query
            .Select(c => new CommentDTO
            {
                commentId = c.CommentId,
                userName = c.User.UserName,
                userImage = c.User.Picture != null ? c.User.Picture.Path : null,
                userId = c.UserId,
                gameId = c.GameId,
                commentText = c.CommentText,
                commentDate = c.CommentDate
            })
            .Take(comment.commentsCount)
            .ToListAsync();
        var commentsFiltrationsCount = await query.CountAsync();

        return new GetCommentsData
        {
            comments = comments,
            CommentsFiltrationsCount = commentsFiltrationsCount
        };
    }


    public async Task<int?> GetUserIdByCommentIdAsync(int CommentId)
    {
        var context = CreateDbContext();

        var result = await context.Comments.Where(c => c.CommentId == CommentId).Select(cc => (int?)cc.UserId).FirstOrDefaultAsync();
        return result;
    }

    public async Task<bool> DeleteCommentAsync(int commentId)
    {
        var context = CreateDbContext();

        var comment = await context.Comments.FindAsync(commentId);

        if (comment == null)
            return false;

        context.Comments.Remove(comment);

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<CommentDTO?> UpdateCommentAsync(CommentPutDTO newComment)
    {
        var context = CreateDbContext();

        var comment = await context.Comments.FindAsync(newComment.commentId);

        if (comment == null)
            return null;

        comment.CommentText = newComment.CommentText;

        await context.SaveChangesAsync();

        return await context.Comments
            .Where(c => c.CommentId == comment.CommentId)
            .Select(c => new CommentDTO
            {
                commentId = c.CommentId,
                userName = c.User.UserName,
                userImage = c.User.Picture != null ? c.User.Picture.Path : null,
                userId = c.UserId,
                gameId = c.GameId,
                commentText = c.CommentText,
                commentDate = c.CommentDate
            })
            .FirstOrDefaultAsync();
    }

}
