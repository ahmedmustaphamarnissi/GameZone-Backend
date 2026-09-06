using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameZoneBack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Data.enums.Sorts;
using Models.DTO;

namespace Data_Access_Layer;

public class ReviewData : BaseData
{
    public ReviewData(IConfiguration config) : base(config)
    {

    }

    public async Task<ReviewDTO?> AddReviewAsync(ReviewPostDTO reviewDTO)
    {
        try
        {
            var review = new Review
            {
                UserId = reviewDTO.userId,
                GameId = reviewDTO.gameId,
                Review1 = reviewDTO.rate,
                ReviewComment = reviewDTO.reviewComment,
                CreatedAt = reviewDTO.createdAt,
            };

            var context = CreateDbContext();
            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var user = await context.Users
                .Where(u => u.UserId == reviewDTO.userId)
                .Select(u => new { u.UserName, PicturePath = u.Picture != null ? u.Picture.Path : null })
                .FirstOrDefaultAsync();

            return new ReviewDTO
            {
                reviewId = review.ReviewId,
                userName = user?.UserName,
                userImage = user?.PicturePath,
                userId = reviewDTO.userId,
                gameId = reviewDTO.gameId,
                rate = reviewDTO.rate,
                reviewComment = reviewDTO.reviewComment,
                createdAt = review.CreatedAt,
            };
        }
        catch (Exception ex)
        {
            throw; // preserves original stack trace instead of `throw ex;`
        }
    }


    public async Task<List<ReviewDTO>?> GetReviewAsync(ReviewsRequest review , int? userId)
    {
        var context = CreateDbContext();

        var query = context.Reviews.Where(r => r.GameId == review.gameId && r.UserId!=userId);

        switch (review.orderBy)
        {
            case ReviewsOrderBy.MostRecent:
                query = query.OrderByDescending(rr => rr.CreatedAt);
                break;
            case ReviewsOrderBy.LowestRated:
                query = query.OrderBy(rr => rr.Review1).ThenByDescending(rr => rr.CreatedAt);
                break;
            case ReviewsOrderBy.HighestRated:
                query = query.OrderByDescending(rr => rr.Review1).ThenByDescending(rr => rr.CreatedAt);
                break;
        }
            var result = await query
            .Select(r => new ReviewDTO
            {
                reviewId = r.ReviewId,
                userName = r.User.UserName,
                gameId = r.GameId,
                userImage = r.User.Picture.Path,
                userId = r.UserId,
                rate = r.Review1,
                reviewComment = r.ReviewComment,
                createdAt = r.CreatedAt,
            }).Take(review.reviewsCount).ToListAsync();
        return result;
    }

    public async Task<int?> GetUserIdByReviewIdAsync(int ReviewId)
    {
        var context = CreateDbContext();

        var result = await context.Reviews.Where(r => r.ReviewId == ReviewId).Select(rr => (int?)rr.UserId).FirstOrDefaultAsync();
        return result;
    }

    public async Task<bool> DeleteReviewByReviewIdAsync(int reviewId)
    {
        var context = CreateDbContext();

        var review = await context.Reviews.FindAsync(reviewId);

        if (review == null)
            return false;

        context.Reviews.Remove(review);

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<ReviewDTO?> UpdateReviewAsync(ReviewsPutDTO newReview)
    {
        var context = CreateDbContext();

        var review = await context.Reviews.FindAsync(newReview.reviewId);

        if (review == null)
            return null;

        review.Review1 = newReview.rate;
        review.ReviewComment = newReview.reviewComment;

        await context.SaveChangesAsync();

        return await context.Reviews
            .Where(r => r.ReviewId == review.ReviewId)
            .Select(r => new ReviewDTO
            {
                reviewId = r.ReviewId,
                userName = r.User.UserName,
                gameId = r.GameId,
                userImage = r.User.Picture.Path,
                userId = r.UserId,
                rate = r.Review1,
                reviewComment = r.ReviewComment,
                createdAt = r.CreatedAt,
            })
            .FirstOrDefaultAsync();
    }

}
