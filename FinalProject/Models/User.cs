using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models;

public class User
{
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string Role { get; set; } = "User";
}
