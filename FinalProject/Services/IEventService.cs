using FinalProject.DTOs;

namespace FinalProject.Services;

public interface IEventService
{
    Task<IReadOnlyList<EventDto>> GetLatestEventsAsync(int take = 8);
}
