using BethanysPieShop.Models;
using BethanysPieShop.Utility;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanysPieShop.Repositories
{
    public class Pie
    {
        public int PieId { get; set; }
        [Remote("CheckPieNameIfAlreadyExists","PieManagement",ErrorMessage ="The name is taken.")]
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string AllergyInformation { get; set; }
        public decimal Price { get; set; }
        [ValidUrl(ErrorMessage ="Invalid Url")]
        public string ImageUrl { get; set; }
        [ValidUrl(ErrorMessage = "Invalid Url")]
        public string ImageThumbnailUrl { get; set; }
        public bool IsPieOfTheWeek { get; set; }
        public bool InStock { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public virtual List<PieReview> PieReviews { get; set; }

    }
}
