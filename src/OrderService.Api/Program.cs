using Microsoft.EntityFrameworkCore;
using OrderService.Api.Data;
using FluentValidation.AspNetCore;
using OrderService.Api.Validators;
using FluentValidation;
using OrderService.Api.RabbitMQ;
using StackExchange.Redis;
using OrderService.Api.Mapping;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<OrderDbContext>(options =>
options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<OrderDtoValidator>();

// Redis baðlantýsý ekleniyor
builder.Services.AddSingleton<RedisCacheService>();
var redisConnection = builder.Configuration.GetSection("Redis:Connection").Value;
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));

// RabbitMQ servisleri ekleniyor
builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
builder.Services.AddSingleton<OrderPublisher>();

// AutoMapper ekleniyor
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();

app.UseAuthorization();
app.UseHttpsRedirection();

app.Run();
