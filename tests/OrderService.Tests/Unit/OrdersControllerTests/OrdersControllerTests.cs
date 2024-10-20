using Microsoft.EntityFrameworkCore;
using OrderService.Api.Controllers;
using OrderService.Api.Data;
using OrderService.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace OrderService.UnitTests
{
    public class OrdersControllerTests
    {
        private readonly OrdersController _controller;
        private readonly OrderDbContext _dbContext;

        public OrdersControllerTests()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderDb")
                .Options;

            _dbContext = new OrderDbContext(options);

            // Test verileri ekle
            _dbContext.Orders.AddRange(new List<Order>
            {
                new Order { Id = 1, CustomerName = "Ali", Product = "Laptop", Price = 15000 },
                new Order { Id = 2, CustomerName = "Veli", Product = "Phone", Price = 8000 }
            });
            _dbContext.SaveChanges();

            _controller = new OrdersController(_dbContext, null, null, null);
        }

        [Fact]
        public async Task GetOrders_ShouldReturnOk_WithOrderList()
        {
            // Act
            var result = await _controller.GetOrders();

            // Assert
            var okResult = result as Microsoft.AspNetCore.Mvc.OkObjectResult;
            okResult.Should().NotBeNull();

            var orders = okResult.Value as List<Order>;
            orders.Should().NotBeNull().And.HaveCount(2);
        }
    }
}
