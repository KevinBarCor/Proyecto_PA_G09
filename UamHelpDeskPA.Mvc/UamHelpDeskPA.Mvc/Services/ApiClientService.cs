using System.Net.Http.Headers;

namespace UamHelpDeskPA.Mvc.Services;

public class ApiClientService(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor)
{
    public HttpClient CreateClient()
    {
        var client = httpClientFactory.CreateClient();

        var token = httpContextAccessor.HttpContext?
            .Request.Cookies["access_token"];

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }
}