namespace CheburechnayaAPI.Models.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int PositionId { get; set; }
        public string PositionTitle { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
    }

    public class EmployeeCreateDto
    {
        public string FullName { get; set; } = string.Empty;
        public int PositionId { get; set; }
        public DateTime HireDate { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class EmployeeUpdateDto
    {
        public string FullName { get; set; } = string.Empty;
        public int PositionId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
