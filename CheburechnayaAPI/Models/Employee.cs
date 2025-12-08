namespace CheburechnayaAPI.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int PositionId { get; set; }
        public DateTime HireDate { get; set; }
        public string PhoneNumber { get; set; }

        public Position Position { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
