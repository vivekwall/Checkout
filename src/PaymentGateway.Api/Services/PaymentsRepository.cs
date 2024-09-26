using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository
{
    public List<PostPaymentResponse> Payments = new();
    
    public virtual void Add(PostPaymentResponse payment)
    {
        Payments.Add(payment);
    }

    public virtual async Task<PostPaymentResponse> Get(Guid id)
    {
        return Payments.FirstOrDefault(p => p.Id == id);
    }
}