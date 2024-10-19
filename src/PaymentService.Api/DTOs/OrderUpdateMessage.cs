using OrderService.Api.Models;

namespace PaymentService.Api.DTOs
{
    public class OrderUpdateMessage
    {
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }
    }
}
