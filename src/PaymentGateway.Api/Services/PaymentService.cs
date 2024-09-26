using Microsoft.Extensions.Options;
using PaymentGateway.Api.Configuration;
using PaymentGateway.Api.Dtos;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services.Extensions;
using PaymentGateway.Api.Services.HttpHelper;

namespace PaymentGateway.Api.Services;

public class PaymentService : IPaymentService
{
    private readonly IHttpClientWrapper _httpClientWrapper;
    private readonly string[] _validCurrencies = { "USD", "EUR", "GBP" };
    private readonly PaymentsRepository _paymentsRepository;
    private readonly ILogger<PaymentService> _logger;
    private readonly BankSimulatorConfig _bankSimulatorConfig;

    public PaymentService(IHttpClientWrapper httpClientWrapper, 
                          PaymentsRepository paymentsRepository, 
                          IOptions<BankSimulatorConfig> bankSimulatorConfig, 
                          ILogger<PaymentService> logger)
    {
        _httpClientWrapper = httpClientWrapper;
        _paymentsRepository = paymentsRepository;
        _logger = logger;
        _bankSimulatorConfig = bankSimulatorConfig.Value;
    }

    public async Task<PostPaymentResponse> Get(Guid id)
    {
        var payment = await _paymentsRepository.Get(id);

        if (payment == null)
        {
            throw new NotFoundException($"Payment with ID {id} not found.");
        }

        return payment;
    }

    public async Task<PostPaymentResponse> ProcessPayment(PaymentRequestDto request)
    {
        if (!_validCurrencies.Contains(request.Currency))
        {
            return request.ToPaymentResponse();
        }

        var currentDate = DateTime.UtcNow;
        var expiryDate = new DateTime(request.ExpiryYear, request.ExpiryMonth, 1);
        if (expiryDate <= currentDate)
        {
            return request.ToPaymentResponse();
        }

        var bankSimulatorUrl = _bankSimulatorConfig.Url;
        var bankSimulatorResponse = await request.SendToBankSimulatorAsync(_httpClientWrapper, bankSimulatorUrl, _logger);

        if (bankSimulatorResponse == null)
        {
            return request.ToPaymentResponse();
        }

        var transationData = new PostPaymentResponse();
      
        transationData.PopulatePaymentResponse(request, bankSimulatorResponse);

        _paymentsRepository.Add(transationData);

        return transationData;
    }
}
