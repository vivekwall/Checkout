using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using Newtonsoft.Json;

using PaymentGateway.Api.Configuration;
using PaymentGateway.Api.Dtos;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Services.HttpHelper;

namespace PaymentGateway.Api.Tests;

public class PaymentServiceUnitTests
{
    private readonly Mock<PaymentsRepository> _mockPaymentsRepository;
    private readonly Mock<IHttpClientWrapper> _mockHttpClientWrapper;
    private readonly Mock<ILogger<PaymentService>> _mockLogger;
    private readonly BankSimulatorConfig _bankSimulatorConfig;
    private readonly PaymentService _paymentService;

    public PaymentServiceUnitTests()
    {
        _mockPaymentsRepository = new Mock<PaymentsRepository>();
        _mockHttpClientWrapper = new Mock<IHttpClientWrapper>(); 
        _mockLogger = new Mock<ILogger<PaymentService>>();
        _bankSimulatorConfig = new BankSimulatorConfig { Url = "http://localhost:8080/payments" };

        var options = Options.Create(_bankSimulatorConfig);
        _paymentService = new PaymentService(
            _mockHttpClientWrapper.Object,
            _mockPaymentsRepository.Object,
            options,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Get_ValidId_ReturnsPostPaymentResponse()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var expectedResponse = new PostPaymentResponse { Id = paymentId };

        _mockPaymentsRepository.Setup(repo => repo.Get(paymentId)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _paymentService.Get(paymentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
    }

    [Fact]
    public async Task ProcessPayment_InvalidCurrency_ReturnsPaymentResponse()
    {
        // Arrange
        var request = new PaymentRequestDto
        {
            CardNumber = "4111111111111111",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Currency = "ABC",  
            Amount = 100,
            CVV = "123"
        };

        // Act
        var result = await _paymentService.ProcessPayment(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Currency, result.Currency);  
    }

    [Fact]
    public async Task ProcessPayment_ExpiredCard_ReturnsPaymentResponse()
    {
        // Arrange
        var request = new PaymentRequestDto
        {
            CardNumber = "4111111111111111",
            ExpiryMonth = 1,
            ExpiryYear = 2020,  
            Currency = "USD",
            Amount = 100,
            CVV = "123"
        };

        // Act
        var result = await _paymentService.ProcessPayment(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Currency, result.Currency);  
    }

    [Fact]
    public async Task ProcessPayment_ValidRequest_CallsBankSimulator()
    {
        // Arrange
        var request = new PaymentRequestDto
        {
            CardNumber = "4111111111111111",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Currency = "USD",
            Amount = 100,
            CVV = "123"
        };


        var bankSimulatorResponse = new { authorized = true, authorization_code = "test-code" };
        _mockHttpClientWrapper.Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
            .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(bankSimulatorResponse)) });

        // Act
        var result = await _paymentService.ProcessPayment(request);

        // Assert
        Assert.NotNull(result);
        _mockPaymentsRepository.Verify(repo => repo.Add(It.IsAny<PostPaymentResponse>()), Times.Once);
    }
}
