using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer;
using GameZoneBack.Models;
using Microsoft.Extensions.Configuration;
using Models.DTO;

namespace Business_Layer
{
    public class clsReview : BaseService
    {
        public clsReview(IConfiguration config) : base(config) { }

        public async Task<ReviewDTO> AddReviewAsync(ReviewPostDTO reviewDTO)
        {
            var data = await new ReviewData(_config).AddReviewAsync(reviewDTO);
            if (data == null)
                throw new ArgumentException("Review is not posted");
            return data;
        }
        public async Task<List<ReviewDTO>?> GetReviewAsync(ReviewsRequest review , int? userId)
        {
            var data = await new ReviewData(_config).GetReviewAsync(review, userId);
            if (data == null)
                throw new ArgumentException("Reviews are not found");
            return data;
        }
        public async Task<int?> GetUserIdByReviewIdAsync(int ReviewId)
        {
            var data = await new ReviewData(_config).GetUserIdByReviewIdAsync(ReviewId);
            return data;
        }
        public async Task<bool> DeleteReviewByReviewIdAsync(int reviewId)
        {
            var data = await new ReviewData(_config).DeleteReviewByReviewIdAsync(reviewId);
            return data;
        }
        public async Task<ReviewDTO> UpdateReviewAsync(ReviewsPutDTO newReview)
        {
            var data = await new ReviewData(_config).UpdateReviewAsync(newReview);
            if (data == null)
                throw new ArgumentException("review can't be updated");
            return data;
        }
    }
}
