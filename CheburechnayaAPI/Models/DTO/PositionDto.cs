namespace CheburechnayaAPI.Models.DTOs
{
    public class PositionDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public int EmployeeCount { get; set; }
    }

    public class PositionCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public decimal Salary { get; set; }
    }
}
