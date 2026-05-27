using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Вкажіть email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;
}
