using FinalProject.Models;
using FinalProject.ViewModels;

namespace FinalProject.Services;

public interface IAuthService
{
    Task<(bool Success, string ErrorMessage, User? CreatedUser)> Register(RegisterViewModel model);
    Task<User?> Login(LoginViewModel model);
}
