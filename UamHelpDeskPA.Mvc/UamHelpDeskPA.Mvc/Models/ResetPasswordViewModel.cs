using System.ComponentModel.DataAnnotations;

namespace UamHelpDeskPA.Mvc.Models;

public class ResetPasswordViewModel
{
    [Required]
    public string SessionToken { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Código OTP")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = string.Empty;
}