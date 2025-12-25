namespace CheburechnayaAPI.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int EmployeeId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string DriverPhone { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";

        public Supplier Supplier { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public List<DeliveryItem> DeliveryItems { get; set; } = new();
    }
}
