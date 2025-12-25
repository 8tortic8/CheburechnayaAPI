namespace CheburechnayaAPI.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int PositionId { get; set; }
        public DateTime HireDate { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;

        public Position? Position { get; set; }
        public List<Order> Orders { get; set; } = new();
    }
}
