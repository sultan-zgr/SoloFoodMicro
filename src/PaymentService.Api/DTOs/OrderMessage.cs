namespace PaymentService.Api.DTOs
{
    public class OrderMessage
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string Product { get; set; }
        public double Price { get; set; }
    }
}
