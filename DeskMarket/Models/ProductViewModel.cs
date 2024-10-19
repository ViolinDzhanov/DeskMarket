using DeskMarket.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static DeskMarket.Common.Constants;

namespace DeskMarket.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(ProductNameMinLength)]
        [MaxLength(ProductNameMaxLength)]
        public string ProductName { get; set; } = null!;

        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } =null!;

        [Range(ProductPriceMinValue, ProductPriceMaxValue)]
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string AddedOn { get; set; } = DateTime.Now.ToString(ProductAddedOnDateFormat);
        public int CategoryId { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
        public string? SellerId { get; set; }
    }
}
