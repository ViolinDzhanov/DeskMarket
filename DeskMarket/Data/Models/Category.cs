using System.ComponentModel.DataAnnotations;
using static DeskMarket.Common.Constants;

namespace DeskMarket.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(CategoryNameMaxLength)]
        public required string Name { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

//•	Has Id – a unique integer, Primary Key
//•	Has Name – a string with min length 3 and max length 20 (required)
//•	Has Products – a collection of type Product
