﻿namespace OrderService.Api.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Product { get; set; }
        public double Price { get; set; }
    }

}
