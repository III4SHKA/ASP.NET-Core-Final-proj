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

    public async Task<IReadOnlyList<EventDto>> GetLatestEventsAsync(int skip = 0, int take = 8)
    {
        return await _dbContext.Events
            .Include(e => e.Category)
            .OrderBy(e => e.StartAt)
            .Skip(skip)
            .Take(take)
            .Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Location = e.Location,
                StartAt = e.StartAt,
                CategoryName = e.Category != null ? e.Category.Name : string.Empty,
                ImageUrl = e.ImageUrl
            })
            .ToListAsync();
    }

    public async Task<int> GetEventsCount()
    {
        return await _dbContext.Events.CountAsync();
    }
}
