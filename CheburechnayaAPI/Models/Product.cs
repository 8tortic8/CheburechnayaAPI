namespace CheburechnayaAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? CostPrice { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
        public List<DeliveryItem> DeliveryItems { get; set; } = new();
    }
}
