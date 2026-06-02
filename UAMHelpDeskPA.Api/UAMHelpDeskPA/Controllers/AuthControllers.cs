using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UamHelpDeskPA.Api.DTOs;

namespace UamHelpDesk.Api.Controllers;

/// <summary>
/// Controlador para autenticación y emisión de tokens JWT.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration configuration) : ControllerBase
{

    /// Genera un JWT válido por 1 hora para acceso a endpoints protegidos.
    /// <param name="request">Credenciales de acceso del usuario.</param>
    /// <returns>Token JWT y datos de expiración.</returns>
    [AllowAnonymous]
    [HttpPost(nameof(Login))]
    [ProducesResponseType(typeof(ApiOperationResultDto<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = "Los datos proporcionados no son válidos."
            });
        }

        var defaultUser = configuration["Jwt:DefaultUser"];
        var defaultPassword = configuration["Jwt:DefaultPassword"];
        var issuer = configuration["Jwt:Issuer"]!;
        var audience = configuration["Jwt:Audience"]!;
        var secretKey = configuration["Jwt:SecretKey"]!;
        var expirationMinutes = configuration.GetValue<int>("Jwt:TokenExpirationMinutes");

        if (!string.Equals(request.Username, defaultUser, StringComparison.Ordinal) ||
            !string.Equals(request.Password, defaultPassword, StringComparison.Ordinal))
        {
            return Unauthorized(new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status401Unauthorized.ToString(),
                Message = "Credenciales inválidas."
            });
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var defaultRole = configuration["Jwt:DefaultRole"] ?? "Admin";

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, request.Username),
            new(JwtRegisteredClaimNames.UniqueName, request.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, defaultRole)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        Console.WriteLine(jwt);
        return Ok(new ApiOperationResultDto<LoginResponseDto>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = "Token generado correctamente.",
            Result = new LoginResponseDto
            {
                AccessToken = jwt,
                ExpiresIn = expirationMinutes * 60
            }
        });

    }
}