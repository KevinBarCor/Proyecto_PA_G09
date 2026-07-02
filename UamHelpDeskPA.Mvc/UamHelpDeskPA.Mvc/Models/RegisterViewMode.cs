using System.ComponentModel.DataAnnotations;

namespace UamHelpDeskPA.Mvc.Models;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Nombre")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Apellido")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password),
        ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = string.Empty;
}