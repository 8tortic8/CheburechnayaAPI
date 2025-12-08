namespace CheburechnayaAPI.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int EmployeeId { get; set; }
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public string VehicleNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        public Supplier Supplier { get; set; }
        public Employee Employee { get; set; }
        public List<DeliveryItem> DeliveryItems { get; set; } = new();
    }
}
