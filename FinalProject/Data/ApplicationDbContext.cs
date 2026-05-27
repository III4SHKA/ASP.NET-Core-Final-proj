using FinalProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Data;

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
