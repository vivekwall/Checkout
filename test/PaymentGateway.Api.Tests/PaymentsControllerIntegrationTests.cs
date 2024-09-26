using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerIntegrationTests
{
    private readonly Random _random = new();
    
    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999),
            Currency = "GBP"
        };

        var paymentsRepository = new PaymentsRepository();
        paymentsRepository.Add(payment);

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)))
            .CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");

        var content = await response.Content.ReadAsStringAsync();

        using var jsonDocument = JsonDocument.Parse(content);
        var resultElement = jsonDocument.RootElement;

        var paymentResponse = new PostPaymentResponse
        {
            Id = Guid.Parse(resultElement.GetProperty("id").GetString()),
            Status = Enum.Parse<PaymentStatus>(resultElement.GetProperty("status").GetString()),
            CardNumberLastFour = resultElement.GetProperty("cardNumberLastFour").GetInt32(),
            ExpiryMonth = resultElement.GetProperty("expiryMonth").GetInt32(),
            ExpiryYear = resultElement.GetProperty("expiryYear").GetInt32(),
            Currency = resultElement.GetProperty("currency").GetString(),
            Amount = resultElement.GetProperty("amount").GetInt32()
        };

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}