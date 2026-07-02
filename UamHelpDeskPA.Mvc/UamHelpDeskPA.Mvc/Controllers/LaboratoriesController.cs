using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using UamHelpDeskPA.Mvc.Models;
using UamHelpDeskPA.Mvc.Services;

namespace UamHelpDeskPA.Mvc.Controllers;

public class LaboratoriesController(
    ApiClientService apiClient,
    IConfiguration configuration) : Controller
{
    private static readonly JsonSerializerOptions JsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetLaboratories(CancellationToken cancellationToken)
    {
        var client = apiClient.CreateClient();

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}" +
            "/GetAllLaboratories";

        var response = await client.GetAsync(endpoint, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, content);

        var result = JsonSerializer.Deserialize<ApiResponse<List<LaboratoryDto>>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return Json(result?.Result ?? new List<LaboratoryDto>());
    }
    [HttpGet]
public async Task<IActionResult> GetLaboratoryById(int id, CancellationToken cancellationToken)
{
    var client = apiClient.CreateClient();

    var endpoint =
        $"{configuration["ApiSettings:BaseUrl"]}" +
        $"{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}" +
        $"/GetLaboratoryById/{id}";

    var response = await client.GetAsync(endpoint, cancellationToken);

    var content = await response.Content.ReadAsStringAsync(cancellationToken);

    if (!response.IsSuccessStatusCode)
        return StatusCode((int)response.StatusCode, content);

    var result = JsonSerializer.Deserialize<ApiResponse<LaboratoryDto>>(content,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    return Json(result?.Result);
}
    [HttpPost]
    public async Task<IActionResult> CreateLaboratory(
    [FromBody] LaboratoryUpsertDto dto,
    CancellationToken cancellationToken)
    {
        var client = apiClient.CreateClient();

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}" +
            "/CreateLaboratory";

        var response = await client.PostAsJsonAsync(endpoint, dto, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return StatusCode((int)response.StatusCode, content);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateLaboratory(
        int id,
        [FromBody] LaboratoryUpsertDto dto,
        CancellationToken cancellationToken)
    {
        var client = apiClient.CreateClient();

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}" +
            $"/UpdateLaboratory/{id}";

        var response = await client.PutAsJsonAsync(endpoint, dto, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteLaboratory(int id, CancellationToken cancellationToken)
    {
        var client = apiClient.CreateClient();

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:LaboratoriesBaseEndpoint"]}" +
            $"/DeleteLaboratory/{id}";

        var response = await client.DeleteAsync(endpoint, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

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
                ApiResponse<AuthResponseDto>>
            (content, JsonOptions);

        return apiResult?.Result?.AccessToken;
    }
}