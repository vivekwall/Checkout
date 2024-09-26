namespace PaymentGateway.Api.Services.HttpHelper;

public interface IHttpClientWrapper
{
    Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
}
