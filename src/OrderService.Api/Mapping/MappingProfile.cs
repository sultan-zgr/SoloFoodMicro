using AutoMapper;
using OrderService.Api.DTOs;
using OrderService.Api.Models;

namespace OrderService.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderDto, Order>();
            CreateMap<Order, OrderDto>();
        }
    }
}
