using FluentValidation;
using OrderService.Api.DTOs;

namespace OrderService.Api.Validators
{
    public class OrderDtoValidator : AbstractValidator<OrderDto>
    {
        public OrderDtoValidator() 
        {
            RuleFor(order => order.CustomerName)
                .NotEmpty().WithMessage("Customer name cannot be empty")
                .Length(2, 50);

            RuleFor(order => order.Product)
                .NotEmpty().WithMessage("Prdouct name cannot be empt");

            RuleFor(order => order.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }
}
