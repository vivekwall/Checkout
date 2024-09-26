using System.Text;
using Newtonsoft.Json;
using PaymentGateway.Api.Dtos;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services.HttpHelper;

namespace PaymentGateway.Api.Services.Extensions;

public static class PaymentExtensions
{
    public static PostPaymentResponse ToPaymentResponse(this PaymentRequestDto request)
    {
        return new PostPaymentResponse
        {
            Status = PaymentStatus.Rejected,
            CardNumberLastFour = 0000,
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency,
            Amount = request.Amount
        };
    }

    public static async Task<BankSimulatorResponse?> SendToBankSimulatorAsync(this PaymentRequestDto request, IHttpClientWrapper httpClientWrapper, string bankSimulatorUrl, ILogger logger)
    {
        var bankRequest = new
        {
            card_number = request.CardNumber,
            expiry_date = $"{request.ExpiryMonth:D2}/{request.ExpiryYear}",
            currency = request.Currency.ToUpper(),
            amount = request.Amount,
            cvv = request.CVV
        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(bankRequest), Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClientWrapper.PostAsync(bankSimulatorUrl, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<BankSimulatorResponse>(content);
            }
            else
            {
                logger.LogError($"Failed to communicate with bank simulator. Status Code: {response.StatusCode}");
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogError($"Error while communicating with the bank simulator: {ex.Message}");
            throw;
        }
    }

    public static void PopulatePaymentResponse(this PostPaymentResponse response, PaymentRequestDto request, BankSimulatorResponse bankSimulatorResponse)
    {
        response.Id = Guid.NewGuid();
        response.CardNumberLastFour = int.Parse(request.CardNumber[^4..]);
        response.Currency = request.Currency;
        response.Amount = request.Amount;
        response.ExpiryMonth = request.ExpiryMonth;
        response.ExpiryYear = request.ExpiryYear;
        response.Status = bankSimulatorResponse.Authorized ? Models.PaymentStatus.Authorized : Models.PaymentStatus.Declined;
    }
}
