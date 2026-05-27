using FinalProject.Data;
using FinalProject.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

// Головна точка входу в застосунок (startup).
// Тут налаштовується інфраструктура проєкту:
// - MVC;
// - підключення БД;
// - DI-сервіси;
// - cookie-авторизація;
// - маршрути.
var builder = WebApplication.CreateBuilder(args);

// Додаємо MVC-контролери + views (Razor).
builder.Services.AddControllersWithViews();

// Беремо рядок підключення до SQLite з appsettings.json.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Реєструємо EF Core контекст.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Реєструємо сервіси бізнес-логіки в DI.
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IHashService, SimpleHashService>();
builder.Services.AddScoped<IAuthService, AuthServiceWithHash>();

// Налаштовуємо cookie-аутентифікацію.
// Якщо користувач не авторизований, його перекине на /Account/Login.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.Name = "podiihub_auth";
    });

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    // Для курсового достатньо EnsureCreated:
    // якщо БД/таблиць ще немає, вони створяться автоматично.
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

// HTTP pipeline (порядок важливий).
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Маршрут за замовчуванням:
// /Home/Index або /{controller}/{action}/{id?}
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
