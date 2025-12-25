namespace CheburechnayaAPI.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public List<Delivery> Deliveries { get; set; } = new();
    }
}
