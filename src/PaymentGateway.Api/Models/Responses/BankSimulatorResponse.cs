namespace PaymentGateway.Api.Models.Responses;

public class BankSimulatorResponse
{
    public bool Authorized { get; set; }
    public string AuthorizationCode { get; set; }
}
