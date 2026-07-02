using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UamHelpDeskPA.Mvc.Models;
using UamHelpDeskPA.Mvc.Services;
namespace UamHelpDeskPA.Mvc.Controllers;

public class RolesController(
    ApiClientService apiClient,
    IConfiguration configuration) : Controller
{
    private static readonly JsonSerializerOptions JsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Edit(int id)
    {
        ViewBag.RoleId = id;
        return View();
    }

    public IActionResult Detail(int id)
    {
        ViewBag.RoleId = id;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
    {
        var client = apiClient.CreateClient();

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:RolesBaseEndpoint"]}" +
            "/GetAllRoles";

        var response = await client.GetAsync(endpoint, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, content);

        var result = JsonSerializer.Deserialize<ApiResponse<List<RoleDto>>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return Json(result?.Result ?? new List<RoleDto>());
    }

    [HttpGet]
    public async Task<IActionResult> GetRoleById(int id, CancellationToken cancellationToken)
    {
        var client = apiClient.CreateClient();

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:RolesBaseEndpoint"]}" +
            $"/GetRoleById/{id}";

        var response = await client.GetAsync(endpoint, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(
    [FromBody] RoleUpsertDto dto,
    CancellationToken cancellationToken)
    {
        var client = apiClient.CreateClient();

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:RolesBaseEndpoint"]}" +
            "/CreateRole";

        var response = await client.PostAsJsonAsync(endpoint, dto, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRole(
    int id,
    [FromBody] RoleUpsertDto dto,
    CancellationToken cancellationToken)
    {
        var client = apiClient.CreateClient();

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:RolesBaseEndpoint"]}" +
            $"/UpdateRole/{id}";

        var response = await client.PutAsJsonAsync(endpoint, dto, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteRole(int id, CancellationToken cancellationToken)
    {
        var client = apiClient.CreateClient();

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:RolesBaseEndpoint"]}" +
            $"/DeleteRole/{id}";

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