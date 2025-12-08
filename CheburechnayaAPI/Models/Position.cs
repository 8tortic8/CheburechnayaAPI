namespace CheburechnayaAPI.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Salary { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
