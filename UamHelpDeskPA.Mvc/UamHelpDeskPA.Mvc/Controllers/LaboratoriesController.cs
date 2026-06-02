using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UamHelpDeskPA.Mvc.Models;

namespace UAMHelpDeskPA.Mvc.Controllers;

public class LaboratoriesController(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) : Controller
{
    private static readonly JsonSerializerOptions JsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetLaboratories(
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();

        var token = await GetTokenAsync(client, cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized(new
            {
                message = "No fue posible autenticar contra el API."
            });
        }

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}" +
            "/GetAllLaboratories";

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            endpoint);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var response =
            await client.SendAsync(request, cancellationToken);

        var content =
            await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, content);
        }

        var apiResult =
            JsonSerializer.Deserialize<
                ApiResponse<List<LaboratoryDto>>>
            (content, JsonOptions);

        return Json(apiResult?.Result ?? new List<LaboratoryDto>());
    }
    [HttpGet]
    public async Task<IActionResult> GetLaboratoryById(
    int id,
    CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();

        var token = await GetTokenAsync(client, cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized(new
            {
                message = "No fue posible autenticar contra el API."
            });
        }

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}" +
            $"/GetLaboratoryById/{id}";

        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var response =
            await client.SendAsync(request, cancellationToken);

        var content =
            await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, content);
        }

        var apiResult =
            JsonSerializer.Deserialize<ApiResponse<LaboratoryDto>>(
                content,
                JsonOptions);

        return Json(apiResult);
    }
    [HttpPost]
    public async Task<IActionResult> CreateLaboratory(
        [FromBody] LaboratoryUpsertDto dto,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();

        var token = await GetTokenAsync(client, cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized(new
            {
                message = "No fue posible autenticar contra el API."
            });
        }

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}" +
            "/CreateLaboratory";

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            endpoint);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        request.Content =
            new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json");

        using var response =
            await client.SendAsync(request, cancellationToken);

        var content =
            await response.Content.ReadAsStringAsync(cancellationToken);

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateLaboratory(
        int id,
        [FromBody] LaboratoryUpsertDto dto,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();

        var token = await GetTokenAsync(client, cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized(new
            {
                message = "No fue posible autenticar contra el API."
            });
        }

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}" +
            $"/UpdateLaboratory/{id}";

        using var request = new HttpRequestMessage(
            HttpMethod.Put,
            endpoint);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        request.Content =
            new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json");

        using var response =
            await client.SendAsync(request, cancellationToken);

        var content =
            await response.Content.ReadAsStringAsync(cancellationToken);

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteLaboratory(
        int id,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();

        var token = await GetTokenAsync(client, cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized(new
            {
                message = "No fue posible autenticar contra el API."
            });
        }

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}" +
            $"/DeleteLaboratory/{id}";

        using var request = new HttpRequestMessage(
            HttpMethod.Delete,
            endpoint);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var response =
            await client.SendAsync(request, cancellationToken);

        var content =
            await response.Content.ReadAsStringAsync(cancellationToken);

        return StatusCode((int)response.StatusCode, content);
    }

    private async Task<string?> GetTokenAsync(
        HttpClient client,
        CancellationToken cancellationToken)
    {
        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:LoginEndpoint"]}";

        var payload = new
        {
            username = configuration["ApiSettings:Username"],
            password = configuration["ApiSettings:Password"]
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            endpoint)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json")
        };

        using var response =
            await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content =
            await response.Content.ReadAsStringAsync(cancellationToken);

        var apiResult =
            JsonSerializer.Deserialize<
                ApiResponse<LoginResponseDto>>
            (content, JsonOptions);

        return apiResult?.Result?.AccessToken;
    }
}