namespace CheburechnayaAPI.Models
{
    public class DeliveryItem
    {
        public int Id { get; set; }
        public int DeliveryId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public string? BatchNumber { get; set; }
        public decimal Subtotal { get; private set; }

        public Delivery Delivery { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
