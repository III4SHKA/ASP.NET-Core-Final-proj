using FinalProject.DTOs;

namespace FinalProject.Services;

public interface IEventService
{
    Task<IReadOnlyList<EventDto>> GetLatestEventsAsync(int skip = 0, int take = 8);
    Task<int> GetEventsCount();
}
