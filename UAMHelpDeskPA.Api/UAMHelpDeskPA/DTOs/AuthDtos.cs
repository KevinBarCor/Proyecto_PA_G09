using System.ComponentModel.DataAnnotations;
namespace UamHelpDeskPA.Api.DTOs
{
    public class LoginRequestDto
    {
        /// <summary>
        /// Email de usuario para iniciar sesión.
        /// </summary>
        [Required]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO de salida para devolver token y metadatos de autenticación.
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// Token JWT emitido por la API.
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;
        /// <summary>
        /// RefreshToken JWT emitido por la API.
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de token.
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Tiempo de expiración en segundos.
        /// </summary>
        public int ExpiresIn { get; set; }
    }
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
    public class RefreshTokenResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public string TokenType { get; set; } = "Bearer";

        public int ExpiresIn { get; set; }
    }
    public class LogoutRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
    public class ResetPasswordRequestDto
    {
        [Required]
        public string SessionToken { get; set; } = string.Empty;

        [Required]
        [StringLength(6)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
    public class ChangePasswordRequestDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
    public record MySessionDto(
        int Id,
        string Token,
        DateTime CreatedAtUtc,
        DateTime ExpiresAtUtc,
        DateTime? RevokedAtUtc,
        string? RevokedReason
    );
}
