using System.ComponentModel.DataAnnotations;
namespace UamHelpDeskPA.Api.DTOs
{
    public class LoginRequestDto
    {
        /// <summary>
        /// Nombre de usuario para iniciar sesión.
        /// </summary>
        [Required]
        public string Username { get; set; } = string.Empty;

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
        /// Tipo de token.
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Tiempo de expiración en segundos.
        /// </summary>
        public int ExpiresIn { get; set; }
    }
}
