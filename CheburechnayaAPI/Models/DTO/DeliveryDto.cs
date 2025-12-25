namespace CheburechnayaAPI.Models.DTOs
{
    public class DeliveryCreateDto
    {
        public int SupplierId { get; set; }
        public int EmployeeId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string DriverPhone { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public string? Status { get; set; }
        public List<DeliveryItemCreateDto> DeliveryItems { get; set; } = new();
    }

    public class DeliveryItemCreateDto
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public string? BatchNumber { get; set; }
    }

    public class DeliveryDto
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public DateTime DeliveryDate { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string DriverPhone { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<DeliveryItemDto> DeliveryItems { get; set; } = new();
    }

    public class DeliveryItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCategory { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public string? BatchNumber { get; set; }
    }

    public class DeliveryStatusUpdateDto
    {
        public string Status { get; set; } = string.Empty;
    }
}