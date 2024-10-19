using OrderService.Api.RabbitMQ;
using PaymentService.Api.RabbitMQ;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddSingleton<PaymentService.Api.RabbitMQ.IRabbitMQConnection,
                              PaymentService.Api.RabbitMQ.RabbitMQConnection>();

builder.Services.AddSingleton<PaymentConsumer>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// RabbitMQ Consumer'ý baþlat
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

