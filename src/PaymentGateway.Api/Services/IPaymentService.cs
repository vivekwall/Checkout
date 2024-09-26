using PaymentGateway.Api.Dtos;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IPaymentService
{
    Task<PostPaymentResponse> ProcessPayment(PaymentRequestDto request);

    Task<PostPaymentResponse> Get(Guid id);
}