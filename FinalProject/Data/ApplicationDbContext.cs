using FinalProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Data;

// Контекст EF Core: через нього йде вся робота з таблицями БД.
// Використовується в сервісах і контролерах через DI.
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<SavedEvent> SavedEvents => Set<SavedEvent>();
    public DbSet<BookedEvent> BookedEvents => Set<BookedEvent>();
}
