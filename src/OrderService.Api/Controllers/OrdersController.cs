﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Api.Data;
using OrderService.Api.DTOs;
using OrderService.Api.Models;
using OrderService.Api.RabbitMQ;

namespace OrderService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly IMapper _mapper;
        private readonly OrderPublisher _publisher;
        private readonly RedisCacheService _cacheService;

        public OrdersController(OrderDbContext context, IMapper mapper, OrderPublisher publisher, RedisCacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
            _publisher = publisher;
            _cacheService = cacheService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders.ToListAsync();
            var ordersDto = _mapper.Map<List<OrderDto>>(orders);

            return Ok(ordersDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            // Redis’ten veriyi çek
            var cachedOrder = await _cacheService.GetCacheAsync<OrderDto>($"Order:{id}");
            if (cachedOrder != null)
            {
                return Ok(cachedOrder);  // Redis'ten gelen veri doğru ise direkt döneriz
            }

            // Veritabanından getir
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            // Veriyi DTO'ya dönüştür ve Redis’e ekle
            var orderDto = _mapper.Map<OrderDto>(order);
            await _cacheService.SetCacheAsync($"Order:{id}", orderDto, TimeSpan.FromMinutes(10));

            return Ok(orderDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Order -> OrderMessage dönüşümünü AutoMapper ile yap
            var orderMessage = _mapper.Map<OrderMessage>(order);
            _publisher.PublishOrderCreated(orderMessage);

            // Redis’e ekle
            await _cacheService.SetCacheAsync($"Order:{order.Id}", orderDto, TimeSpan.FromMinutes(10));

            return Ok(order);
        }

    }
}
