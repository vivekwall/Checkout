using System.Text.Json.Serialization;
using System.Text.Json;

using PaymentGateway.Api.Configuration;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Services.HttpHelper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<PaymentService>();


builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<BankSimulatorConfig>(builder.Configuration.GetSection("BankSimulator"));
builder.Services.AddSingleton<PaymentsRepository>();
builder.Services.AddScoped<IHttpClientWrapper, HttpClientWrapper>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
