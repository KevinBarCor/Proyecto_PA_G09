using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using UamHelpDeskPA.Mvc.Models;

namespace UamHelpDeskPA.Mvc.Controllers;

public class AuthController(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) : Controller
{
    private static readonly JsonSerializerOptions JsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = httpClientFactory.CreateClient();

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}" +
            $"{configuration["ApiSettings:LoginEndpoint"]}";

        var response = await client.PostAsJsonAsync(
            endpoint,
            model,
            cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        var result =
            JsonSerializer.Deserialize<ApiResponse<LoginOtpResponseDto>>(
                content,
                JsonOptions);

        if (result == null || !result.Success || result.Result == null)
        {
            ModelState.AddModelError("", "Credenciales inválidas");
            return View(model);
        }

        HttpContext.Session.SetString(
    "SessionToken",
    result.Result.SessionToken);

        return RedirectToAction(nameof(VerifyOtp));
    }
    [HttpGet]
    public IActionResult VerifyOtp()
    {
        var sessionToken =
            HttpContext.Session.GetString("SessionToken");

        if (string.IsNullOrEmpty(sessionToken))
        {
            return RedirectToAction(nameof(Login));
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> VerifyOtp(
    VerifyOtpViewModel model,
    CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var sessionToken =
            HttpContext.Session.GetString("SessionToken");

        if (string.IsNullOrEmpty(sessionToken))
        {
            return RedirectToAction(nameof(Login));
        }

        var client = httpClientFactory.CreateClient();

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}/api/Auth/VerifyOtp";

        var response = await client.PostAsJsonAsync(
            endpoint,
            new
            {
                SessionToken = sessionToken,
                Code = model.Code
            },
            cancellationToken);

        var content =
            await response.Content.ReadAsStringAsync(cancellationToken);

        var result =
            JsonSerializer.Deserialize<ApiResponse<AuthResponseDto>>(
                content,
                JsonOptions);

        if (result == null ||
            !result.Success ||
            result.Result == null)
        {
            ModelState.AddModelError("", "Código OTP inválido.");
            return View(model);
        }

        SetCookies(result.Result);

        HttpContext.Session.Remove("SessionToken");

        return RedirectToAction("Index", "Home");
    }

    private void SetCookies(AuthResponseDto tokens)
    {
        Response.Cookies.Append("access_token", tokens.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(60)
        });

        Response.Cookies.Append("refresh_token", tokens.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }

    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            var client = httpClientFactory.CreateClient();

            var endpoint =
                $"{configuration["ApiSettings:BaseUrl"]}/api/Auth/Logout";

            await client.PostAsJsonAsync(
                endpoint,
                new { refreshToken },
                cancellationToken);
        }

        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");

        return RedirectToAction("Login");
    }
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(
    ForgotPasswordViewModel model,
    CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = httpClientFactory.CreateClient();

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}/api/Auth/ForgotPassword";

        var response = await client.PostAsJsonAsync(
            endpoint,
            model,
            cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        var result =
            JsonSerializer.Deserialize<ApiResponse<object>>(
                content,
                JsonOptions);

        ViewBag.Message = result?.Message;

        return View();
    }
    [HttpGet]
    public IActionResult ResetPassword(string sessionToken)
    {
        Console.WriteLine("ENTRÓ A RESET PASSWORD");
        Console.WriteLine($"TOKEN: {sessionToken}");
        var model = new ResetPasswordViewModel
        {
            SessionToken = sessionToken
        };

        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> ResetPassword(
    ResetPasswordViewModel model,
    CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = httpClientFactory.CreateClient();

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}/api/Auth/ResetPassword";

        var response = await client.PostAsJsonAsync(
            endpoint,
            model,
            cancellationToken);

        var content =
            await response.Content.ReadAsStringAsync(cancellationToken);

        var result =
            JsonSerializer.Deserialize<ApiResponse<object>>(
                content,
                JsonOptions);

        if (result == null || !result.Success)
        {
            ModelState.AddModelError(
                string.Empty,
                result?.Message ?? "No fue posible restablecer la contraseña.");

            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;

        return RedirectToAction(nameof(Login));
    }
    private string? GetAccessToken()
    {
        return Request.Cookies["access_token"];
    }
    public async Task<IActionResult> MySessions(CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();

        AddBearerToken(client);

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}/api/Auth/MySessions";

        var response = await client.GetAsync(endpoint, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "No se pudieron cargar las sesiones.";
            return View(new List<MySessionViewModel>());
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        var result = JsonSerializer.Deserialize<ApiResponse<List<MySessionViewModel>>>(
            content,
            JsonOptions);

        return View(result?.Result ?? new List<MySessionViewModel>());
    }
    private void AddBearerToken(HttpClient client)
    {
        var token = Request.Cookies["access_token"];

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
    [HttpPost]
    public async Task<IActionResult> RevokeSession(int refreshTokenId, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}/api/Auth/RevokeSession/{refreshTokenId}";

        var token = Request.Cookies["access_token"];

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        await client.PostAsync(endpoint, null, cancellationToken);

        return RedirectToAction(nameof(MySessions));
    }
    [HttpPost]
    public async Task<IActionResult> RevokeAllSessions(CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();

        AddBearerToken(client);

        var endpoint =
            $"{configuration["ApiSettings:BaseUrl"]}/api/Auth/RevokeAllSessions";

        await client.PostAsync(endpoint, null, cancellationToken);
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Auth");
    }
}