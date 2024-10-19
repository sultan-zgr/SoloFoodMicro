namespace OrderService.Api.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Product { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = "Pending"; // Varsayılan durum
    }
}
