namespace CheburechnayaAPI.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }

        public List<Delivery> Deliveries { get; set; } = new();
    }
}
