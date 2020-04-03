using BethanysPieShop.Models;
using System.Collections.Generic;

namespace BethanysPieShop.Repositories
{
    public interface IPieReviewRepository
    {
        void AddPieReview(PieReview pieReview);
        IEnumerable<PieReview> GetReviewsForPie(int pieId);
    }
}