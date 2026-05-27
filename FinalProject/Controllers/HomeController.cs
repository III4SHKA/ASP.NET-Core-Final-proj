using FinalProject.Data;
using FinalProject.DTOs;
using FinalProject.Services;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers;

public class HomeController : Controller
{
    private readonly IEventService _eventService;
    private readonly ApplicationDbContext _dbContext;

    public HomeController(IEventService eventService, ApplicationDbContext dbContext)
    {
        _eventService = eventService;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index(int skip = 0, int take = 8, string? category = null, string? searching = null)
    {
        if (skip < 0) skip = 0;
        if (take < 8) take = 8;
        if (take > 64) take = 64;

        var eventsFromDb = await _eventService.GetLatestEventsAsync(skip, take, category, searching);
        var upcomingEvents = await _eventService.GetUpcomingEvents(3);
        var totalEventsCount = await _eventService.GetEventsCount(category, searching);
        var categories = await _dbContext.Categories
            .OrderBy(category => category.Name)
            .Select(category => category.Name)
            .ToListAsync();

        var pageModel = new HomeIndexViewModel
        {
            Events = eventsFromDb.Select(ToCard).ToList(),
            UpcomingEvents = upcomingEvents.Select(ToUpcoming).ToList(),
            Categories = categories,
            SelectedCategory = category,
            SearchQuery = searching ?? string.Empty,
            Skip = skip,
            Take = take,
            TotalCount = totalEventsCount
        };

        return View(pageModel);
    }

    [HttpGet]
    public async Task<IActionResult> LoadMore(int skip = 0, int take = 8, string? category = null, string? searching = null)
    {
        if (skip < 0) skip = 0;
        if (take < 1) take = 8;
        if (take > 64) take = 64;

        var eventsFromDb = await _eventService.GetLatestEventsAsync(skip, take, category, searching);
        var totalEventsCount = await _eventService.GetEventsCount(category, searching);

        var chunkModel = new EventsChunkViewModel
        {
            Events = eventsFromDb.Select(ToCard).ToList(),
            NextSkip = skip + take,
            HasMore = (skip + take) < totalEventsCount
        };

        return PartialView("EventsChunk", chunkModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var eventData = await _eventService.GetEventById(id);
        if (eventData is null)
        {
            return NotFound();
        }

        var detailsModel = new EventDetailsViewModel
        {
            Id = eventData.Id,
            Title = eventData.Title,
            Description = eventData.Description,
            Location = eventData.Location,
            StartAt = eventData.StartAt,
            Capacity = eventData.Capacity,
            ImageUrl = eventData.ImageUrl
        };

        return View(detailsModel);
    }

    private static HomeEventCardViewModel ToCard(EventDto eventData)
    {
        return new HomeEventCardViewModel
        {
            Id = eventData.Id,
            Title = eventData.Title,
            TitleDescription = eventData.TitleDescription,
            Description = eventData.Description,
            Location = eventData.Location,
            StartAt = eventData.StartAt,
            Capacity = eventData.Capacity,
            CategoryName = eventData.CategoryName,
            ImageUrl = eventData.ImageUrl
        };
    }

    private static UpcomingEventViewModel ToUpcoming(EventDto eventData)
    {
        return new UpcomingEventViewModel
        {
            Id = eventData.Id,
            Title = eventData.Title,
            Location = eventData.Location,
            StartAt = eventData.StartAt
        };
    }
}
