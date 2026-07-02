using System.ComponentModel.DataAnnotations;

namespace UamHelpDeskPA.Api.DTOs
{
    public class VerifyOtpRequestDto
    {
        [Required]
        public string SessionToken { get; set; } = string.Empty;

        [Required]
        public string Code { get; set; } = string.Empty;
    }
}
