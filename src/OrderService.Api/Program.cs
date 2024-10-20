using Microsoft.EntityFrameworkCore;
using OrderService.Api.Data;
using FluentValidation.AspNetCore;
using OrderService.Api.Validators;
using OrderService.Api.RabbitMQ;
using StackExchange.Redis;
using OrderService.Api.Mapping;
using RabbitMQ.Client;
using Serilog;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<OrderDtoValidator>();

// Redis Baðlantýsý
var redisConnection = builder.Configuration.GetSection("Redis:Connection").Value;
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    try
    {
        var connection = ConnectionMultiplexer.Connect(redisConnection);
        Log.Information("Connected to Redis.");
        return connection;
    }
    catch (Exception ex)
    {
        Log.Error($"Redis connection failed: {ex.Message}");
        throw;
    }
});

// RabbitMQ Baðlantýsý
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    return new ConnectionFactory
    {
        HostName = "localhost",
        UserName = "guest",
        Password = "guest",
        Port = 5672
    };
});

builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
builder.Services.AddSingleton<OrderPublisher>();
builder.Services.AddSingleton<RedisCacheService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
