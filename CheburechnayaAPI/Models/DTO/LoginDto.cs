namespace CheburechnayaAPI.Models.DTOs
{
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public string Position { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
    }
}
