using PaymentService.Api.RabbitMQ;
using Serilog;
using StackExchange.Redis;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Redis Ba�lant�s�
var redisConnection = builder.Configuration.GetConnectionString("Redis");
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

// RabbitMQ Ba�lant�s�
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var rabbitConfig = builder.Configuration.GetSection("RabbitMQ");
    return new ConnectionFactory
    {
        HostName = rabbitConfig["HostName"],
        UserName = rabbitConfig["UserName"],
        Password = rabbitConfig["Password"],
        Port = int.Parse(rabbitConfig["Port"])
    };
});

builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
builder.Services.AddSingleton<PaymentConsumer>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// RabbitMQ Consumer'� ba�lat
var paymentConsumer = app.Services.GetRequiredService<PaymentConsumer>();
Task.Run(() => paymentConsumer.StartListening());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
