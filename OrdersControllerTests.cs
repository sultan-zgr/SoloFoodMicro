using Microsoft.EntityFrameworkCore;
using OrderService.Api.Controllers;
using OrderService.Api.Data;
using OrderService.Api.Models;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;

namespace OrderService.Tests.Unit
{
    public class OrdersControllerTests
    {
        private readonly OrderDbContext _dbContext;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            // DbContext yapılandırma
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderDb")
                .Options;

            _dbContext = new OrderDbContext(options);

            // Controller oluşturma
            _controller = new OrdersController(_dbContext, null, null, null);
        }

        [Fact]
        public async Task GetOrders_ShouldReturnOk_WithOrderList()
        {
            // Arrange
            _dbContext.Orders.AddRange(new List<Order>
            {
                new Order { CustomerName = "Ali", Product = "Laptop", Price = 15000 },
                new Order { CustomerName = "Veli", Product = "Phone", Price = 8000 }
            });
            await _dbContext.SaveChangesAsync();

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
