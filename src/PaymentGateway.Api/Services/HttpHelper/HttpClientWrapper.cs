namespace PaymentGateway.Api.Services.HttpHelper;

public class HttpClientWrapper : IHttpClientWrapper
{
    private readonly HttpClient _httpClient;

    public HttpClientWrapper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
    {
        return _httpClient.PostAsync(requestUri, content);
    }
}
