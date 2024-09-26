using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Dtos;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services.Extensions;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerUnitTests
{
    private readonly PaymentsController _controller;
    private readonly Mock<IPaymentService> _mockPaymentService;

    public PaymentsControllerUnitTests()
    {
        _mockPaymentService = new Mock<IPaymentService>();
        _controller = new  PaymentsController(_mockPaymentService.Object);
    }

    [Fact]
    public async Task ProcessPayment_ValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new PaymentRequestDto
        {
            CardNumber = "1234567890123456",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Currency = "USD",
            Amount = 100,
            CVV = "123"
        };

        var paymentResponse = new Models.Responses.PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Authorized,
            Amount = 100,
            Currency = "USD",
            CardNumberLastFour = 3456
        };
        _mockPaymentService
            .Setup(x => x.ProcessPayment(It.IsAny<PaymentRequestDto>()))
            .ReturnsAsync(paymentResponse);

        // Act
        var result = await _controller.ProcessPayment(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PostPaymentResponse>(okResult.Value);
        Assert.Equal(paymentResponse.Status, returnValue.Status);
    }

    [Fact]
    public async Task ProcessPayment_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("CardNumber", "Required");

        var request = new PaymentRequestDto
        {
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Currency = "USD",
            Amount = 100,
            CVV = "123"
        };

        // Act
        var result = await _controller.ProcessPayment(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
    }

    [Fact]
    public async Task GetPaymentAsync_ValidId_ReturnsOk()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var paymentResponse = new PostPaymentResponse
        {
            Id = paymentId,
            Status = PaymentStatus.Authorized,
            Amount = 100,
            Currency = "USD",
            CardNumberLastFour = 3456,
            ExpiryMonth = 12,
            ExpiryYear = 2025
        };

        _mockPaymentService.Setup(x => x.Get(paymentId))
            .ReturnsAsync(paymentResponse);

        // Act
        var result = await _controller.GetPaymentAsync(paymentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPaymentAsync_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var paymentId = Guid.NewGuid();


        _mockPaymentService.Setup(x => x.Get(paymentId))
            .ThrowsAsync(new NotFoundException($"Payment with ID {paymentId} not found."));

        // Act
        var result = await _controller.GetPaymentAsync(paymentId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);   
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.Equal($"Payment with ID {paymentId} not found.", notFoundResult?.Value);   
    }
}
