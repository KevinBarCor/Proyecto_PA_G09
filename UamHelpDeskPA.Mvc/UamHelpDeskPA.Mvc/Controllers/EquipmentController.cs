using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UamHelpDeskPA.Mvc.Models;

namespace UAMHelpDeskPA.Mvc.Controllers;

public class EquipmentController(
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
    public async Task<IActionResult> GetEquipment(
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
            $"{configuration["ApiSettings:EquipmentBaseEndpoint"]}" +
            "/GetAllEquipment";

        using var request =
            new HttpRequestMessage(HttpMethod.Get, endpoint);

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
                ApiResponse<List<EquipmentDto>>>
            (content, JsonOptions);

        return Json(apiResult?.Result ?? new List<EquipmentDto>());
    }
    [HttpGet]
    public async Task<IActionResult> GetEquipmentById(
    int id,
    CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();

        var token = await GetTokenAsync(client, cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized();
        }

        var endpoint = $"{configuration["ApiSettings:BaseUrl"]}" + $"{configuration["ApiSettings:EquipmentBaseEndpoint"]}" + $"/GetEquipmentById/{id}";

        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.SendAsync(request, cancellationToken);

        var content =  await response.Content.ReadAsStringAsync(cancellationToken);

        return StatusCode((int)response.StatusCode, content);
    }
    [HttpGet]
    public async Task<IActionResult> GetLaboratories(
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();

        var token = await GetTokenAsync(client, cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized();
        }

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}" +
            "/GetAllLaboratories";

        using var request =
            new HttpRequestMessage(HttpMethod.Get, endpoint);

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

    [HttpPost]
    public async Task<IActionResult> CreateEquipment(
        [FromBody] EquipmentUpsertDto dto,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();

        var token = await GetTokenAsync(client, cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized();
        }

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:EquipmentBaseEndpoint"]}" +
            "/CreateEquipment";

        using var request =
            new HttpRequestMessage(HttpMethod.Post, endpoint);

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
    public async Task<IActionResult> UpdateEquipment(
        int id,
        [FromBody] EquipmentUpsertDto dto,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();

        var token = await GetTokenAsync(client, cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized();
        }

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:EquipmentBaseEndpoint"]}" +
            $"/UpdateEquipment/{id}";

        using var request =
            new HttpRequestMessage(HttpMethod.Put, endpoint);

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
    public async Task<IActionResult> DeleteEquipment(
        int id,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();

        var token = await GetTokenAsync(client, cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized();
        }

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:EquipmentBaseEndpoint"]}" +
            $"/DeleteEquipment/{id}";

        using var request =
            new HttpRequestMessage(HttpMethod.Delete, endpoint);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var response =
            await client.SendAsync(request, cancellationToken);

        var content =
            await response.Content.ReadAsStringAsync(cancellationToken);

        return StatusCode((int)response.StatusCode, content);
    }
    public IActionResult Details(int id)
    {
        ViewBag.EquipmentId = id;

        return View();
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