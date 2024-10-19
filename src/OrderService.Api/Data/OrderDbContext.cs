using Microsoft.EntityFrameworkCore;
using OrderService.Api.DTOs;
using OrderService.Api.Models;

namespace OrderService.Api.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
    }
}
