using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UamHelpDeskPA.Api.DTOs;
using UamHelpDeskPA.Api.Services.Auth;

namespace UamHelpDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    // ===================== AUTH =====================

    [AllowAnonymous]
    [HttpPost(nameof(Login))]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);
        return StatusCode(int.Parse(result.Code), result);
    }

    [AllowAnonymous]
    [HttpPost(nameof(VerifyOtp))]
    public async Task<IActionResult> VerifyOtp(VerifyOtpRequestDto request)
    {
        var result = await _authService.VerifyOtpAsync(request);
        return StatusCode(int.Parse(result.Code), result);
    }

    [AllowAnonymous]
    [HttpPost(nameof(RefreshToken))]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        return StatusCode(int.Parse(result.Code), result);
    }

    [AllowAnonymous]
    [HttpPost(nameof(Logout))]
    public async Task<IActionResult> Logout(LogoutRequestDto request)
    {
        var result = await _authService.LogoutAsync(request);
        return StatusCode(int.Parse(result.Code), result);
    }

    // ===================== PASSWORD RESET =====================

    [AllowAnonymous]
    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto request)
    {
        var result = await _authService.ForgotPasswordAsync(request);
        return StatusCode(int.Parse(result.Code), result);
    }

    [AllowAnonymous]
    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        return StatusCode(int.Parse(result.Code), result);
    }

    // ===================== PASSWORD CHANGE =====================

    [Authorize]
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var currentRefreshToken =
            Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", "");

        var result = await _authService.ChangePasswordAsync(
            userId,
            currentRefreshToken,
            request);

        return StatusCode(int.Parse(result.Code), result);
    }

    // ===================== SESSIONS =====================

    [Authorize]
    [HttpGet("MySessions")]
    public async Task<IActionResult> MySessions()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _authService.GetMySessionsAsync(userId);

        return StatusCode(int.Parse(result.Code), result);
    }

    [Authorize]
    [HttpPost("RevokeSession/{refreshTokenId}")]
    public async Task<IActionResult> RevokeSession(int refreshTokenId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _authService.RevokeSessionAsync(userId, refreshTokenId);

        return StatusCode(int.Parse(result.Code), result);
    }

    [Authorize]
    [HttpPost("RevokeAllSessions")]
    public async Task<IActionResult> RevokeAllSessions()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _authService.RevokeAllSessionsAsync(userId);

        return StatusCode(int.Parse(result.Code), result);
    }
}