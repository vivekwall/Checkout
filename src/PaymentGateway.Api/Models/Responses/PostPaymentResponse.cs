using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Responses;

public class PostPaymentResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("status")]
    public PaymentStatus Status { get; set; }
    [JsonPropertyName("cardNumberLastFour")]
    public int CardNumberLastFour { get; set; }
    [JsonPropertyName("expiryMonth")]
    public int ExpiryMonth { get; set; }

    [JsonPropertyName("expiryYear")]
    public int ExpiryYear { get; set; }
    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("amount")] 
    public int Amount { get; set; }
}
