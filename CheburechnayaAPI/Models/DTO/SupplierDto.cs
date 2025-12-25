namespace CheburechnayaAPI.Models.DTOs
{
    public class SupplierDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int TotalDeliveries { get; set; }
        public decimal TotalDeliveredAmount { get; set; }
    }

    public class SupplierCreateDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
