namespace OrderService.Api.DTOs
{
    public class OrderDto
    {
        public string CustomerName { get; set; }
        public string Product { get; set; }
        public decimal Price { get; set; }
    }
}
