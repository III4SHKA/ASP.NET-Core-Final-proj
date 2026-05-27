using System.Security.Claims;
using FinalProject.Data;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers;

// Контролер адмін-панелі.
// Цей контролер відповідає за просте адміністрування у межах курсового:
// 1) створення події;
// 2) перегляд списку користувачів;
// 3) видача ролі Organizer.
// Весь контролер закритий атрибутом [Authorize(Roles = "Admin")],
// тобто будь-яка дія всередині доступна тільки адміну.
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
        // [HttpGet("")] означає:
        // - HTTP метод: GET;
        // - порожній підмаршрут відносно [Route("admin")].
        // Підсумковий URL цієї дії: /admin
        var model = new AdminPageViewModel();
        await FillUsers(model);
        return View(model);
    }

    // [HttpPost("add-event")] означає:
    // - HTTP метод: POST;
    // - підмаршрут "add-event" відносно [Route("admin")].
    // Підсумковий URL цієї дії: POST /admin/add-event
    // Саме цю адресу викликає форма створення події в адмінці.
    [HttpPost("add-event")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddEvent(AdminPageViewModel model)
    {
        // Приймаємо дані з форми, перевіряємо їх і створюємо Event у БД.
        if (!ModelState.IsValid)
        {
            // Якщо валідація не пройшла, повертаємо цю ж сторінку з помилками.
            await FillUsers(model);
            return View("Index", model);
        }

        if (!model.StartAt.HasValue)
        {
            ModelState.AddModelError(nameof(model.StartAt), "Вкажи дату і час початку.");
            await FillUsers(model);
            return View("Index", model);
        }

        // Беремо id поточного адміна з Claims, щоб записати хто створив подію.
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
        // Видача ролі Organizer конкретному користувачу із таблиці.
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
        // Підтягує список користувачів для таблиці на сторінці /admin.
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
