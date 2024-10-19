using DeskMarket.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static DeskMarket.Common.Constants;

namespace DeskMarket.Models
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }    
        public string ProductName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string AddedOn { get; set; } = DateTime.Now.ToString(ProductAddedOnDateFormat);
        public string CategoryName { get; set; } =null!;

        public string Seller { get; set; } = null!;
        public bool HasBought { get; set; }
    }
}
