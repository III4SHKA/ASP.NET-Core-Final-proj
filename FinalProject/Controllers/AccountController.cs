using System.Security.Claims;
using FinalProject.Data;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly ApplicationDbContext _dbContext;

    public AccountController(IAuthService authService, ApplicationDbContext dbContext)
    {
        _authService = authService;
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var registerResult = await _authService.Register(model);

        if (!registerResult.Success || registerResult.CreatedUser == null)
        {
            ModelState.AddModelError(string.Empty, registerResult.ErrorMessage);
            return View(model);
        }

        await SignInUser(registerResult.CreatedUser);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _authService.Login(model);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Неправильний email або пароль");
            return View(model);
        }

        await SignInUser(user);
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var savedEvents = await _dbContext.SavedEvents
            .Include(savedEvent => savedEvent.Event)
            .Where(savedEvent => savedEvent.UserId == userId && savedEvent.Event != null)
            .OrderByDescending(savedEvent => savedEvent.SavedAt)
            .Select(savedEvent => new SavedEventProfileItemViewModel
            {
                EventId = savedEvent.EventId,
                Title = savedEvent.Event!.Title,
                Location = savedEvent.Event.Location,
                StartAt = savedEvent.Event.StartAt,
                SavedAt = savedEvent.SavedAt
            })
            .ToListAsync();

        var bookedEvents = await _dbContext.BookedEvents
            .Include(bookedEvent => bookedEvent.Event)
            .Where(bookedEvent => bookedEvent.UserId == userId && bookedEvent.Event != null)
            .OrderByDescending(bookedEvent => bookedEvent.BookedAt)
            .Select(bookedEvent => new BookedEventProfileItemViewModel
            {
                EventId = bookedEvent.EventId,
                Title = bookedEvent.Event!.Title,
                Location = bookedEvent.Event.Location,
                StartAt = bookedEvent.Event.StartAt,
                BookedAt = bookedEvent.BookedAt
            })
            .ToListAsync();

        var profileModel = new ProfileViewModel
        {
            Name = User.Identity!.Name!,
            Email = User.FindFirstValue(ClaimTypes.Email)!,
            Role = User.FindFirstValue(ClaimTypes.Role)!,
            SavedEvents = savedEvents,
            BookedEvents = bookedEvents
        };

        return View(profileModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInUser(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
