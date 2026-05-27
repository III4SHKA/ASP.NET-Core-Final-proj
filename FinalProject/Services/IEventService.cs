using FinalProject.DTOs;

namespace FinalProject.Services;

public interface IEventService
{
    Task<IReadOnlyList<EventDto>> GetLatestEventsAsync(int skip = 0, int take = 8, string? category = null, string? searching = null);
    Task<int> GetEventsCount(string? category = null, string? searching = null);
    Task<IReadOnlyList<EventDto>> GetUpcomingEvents(int count = 3);
    Task<EventDto?> GetEventById(int id);
}
