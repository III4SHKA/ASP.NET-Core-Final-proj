using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels;

public class RegisterViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Ім'я")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Паролі не співпадають")]
    [Display(Name = "Підтвердження пароля")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
