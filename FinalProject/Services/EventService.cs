using FinalProject.Data;
using FinalProject.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Services;

public class EventService : IEventService
{
    private readonly ApplicationDbContext _dbContext;

    public EventService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<EventDto>> GetLatestEventsAsync(int skip = 0, int take = 8, string? searching = null)
    {
        var query = _dbContext.Events.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searching))
        {
            var normalizedSearching = searching.Trim().ToLower();
            query = query.Where(eventEntity => eventEntity.Title.ToLower().Contains(normalizedSearching));
        }

        return await query
            .OrderBy(e => e.StartAt)
            .Skip(skip)
            .Take(take)
            .Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                TitleDescription = e.TitleDescription,
                Location = e.Location,
                StartAt = e.StartAt,
                Capacity = e.Capacity,
                ImageUrl = e.ImageUrl
            })
            .ToListAsync();
    }

    public async Task<int> GetEventsCount(string? searching = null)
    {
        var query = _dbContext.Events.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searching))
        {
            var normalizedSearching = searching.Trim().ToLower();
            query = query.Where(eventEntity => eventEntity.Title.ToLower().Contains(normalizedSearching));
        }

        return await query.CountAsync();
    }

    public async Task<IReadOnlyList<EventDto>> GetUpcomingEvents(int count = 3)
    {
        return await _dbContext.Events
            .OrderBy(e => e.StartAt)
            .Take(count)
            .Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                TitleDescription = e.TitleDescription,
                Location = e.Location,
                StartAt = e.StartAt,
                Capacity = e.Capacity,
                ImageUrl = e.ImageUrl
            })
            .ToListAsync();
    }

    public async Task<EventDto?> GetEventById(int id)
    {
        return await _dbContext.Events
            .Where(e => e.Id == id)
            .Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                TitleDescription = e.TitleDescription,
                Location = e.Location,
                StartAt = e.StartAt,
                Capacity = e.Capacity,
                ImageUrl = e.ImageUrl
            })
            .FirstOrDefaultAsync();
    }
}
