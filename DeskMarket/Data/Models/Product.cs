using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static DeskMarket.Common.Constants;

namespace DeskMarket.Data.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ProductNameMaxLength)]
        public string ProductName { get; set; } = null!;

        [Required]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;

        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        [Required]
        public string SellerId { get; set; } = null!;

        [Required]
        public IdentityUser Seller { get; set; } = null!;

        public required DateTime AddedOn { get; set; }
        public required int CategoryId { get; set; }

        [Required]
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;
        public ICollection<ProductClient>? ProductsClients { get; set; } = new List<ProductClient>();
    }
}

