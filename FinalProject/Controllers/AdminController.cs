using System.Security.Claims;
using FinalProject.Data;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers;

[Authorize(Roles = "Admin")]
[Route("admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public AdminController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var model = new AdminPageViewModel();
        await FillUsers(model);
        return View(model);
    }

    [HttpPost("add-event")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddEvent(AdminPageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await FillUsers(model);
            return View("Index", model);
        }

        if (!model.StartAt.HasValue)
        {
            ModelState.AddModelError(nameof(model.StartAt), "Вкажи дату і час початку.");
            await FillUsers(model);
            return View("Index", model);
        }

        var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var eventEntity = new Event
        {
            Title = model.Title.Trim(),
            TitleDescription = model.TitleDescription.Trim(),
            Description = model.Description.Trim(),
            Location = model.Location.Trim(),
            StartAt = model.StartAt.Value,
            Capacity = model.Capacity,
            ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim(),
            OrganizerId = adminId
        };

        _dbContext.Events.Add(eventEntity);
        await _dbContext.SaveChangesAsync();

        TempData["AdminMessage"] = "Подію створено";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("make-organizer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakeOrganizer(int userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
        {
            return RedirectToAction(nameof(Index));
        }

        if (!string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            user.Role = "Organizer";
            await _dbContext.SaveChangesAsync();
        }

        TempData["AdminMessage"] = "Роль оновлено";
        return RedirectToAction(nameof(Index));
    }

    private async Task FillUsers(AdminPageViewModel model)
    {
        model.Users = await _dbContext.Users
            .OrderBy(x => x.Id)
            .Select(x => new AdminUserRowViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Role = x.Role
            })
            .ToListAsync();
    }
}
