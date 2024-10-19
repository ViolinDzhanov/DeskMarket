namespace DeskMarket.Models
{
    public class DeleteProductViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public string SellerId { get; set; } = null!;
        public string Seller { get; set; }
    }
}
