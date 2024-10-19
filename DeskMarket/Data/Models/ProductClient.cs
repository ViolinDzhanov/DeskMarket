using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace DeskMarket.Data.Models
{
    public class ProductClient
    {
        public int ProductId { get; set; }

        [Required]
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
        public required string ClientId { get; set; }

        [Required]
        [ForeignKey(nameof(ClientId))]
        public IdentityUser Client { get; set; } = null!;
    }
}
//•	Has ProductId – integer, PrimaryKey, foreign key (required)
//•	Has Product – Product
//•	Has ClientId – string, PrimaryKey, foreign key (required)
//•	Has Client – IdentityUser
