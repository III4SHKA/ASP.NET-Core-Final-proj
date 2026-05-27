using FinalProject.DTOs;
using FinalProject.Services;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventService _eventService;

        public HomeController(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<IActionResult> Index(int skip = 0, int take = 8)
        {
            if (skip < 0) skip = 0;
            if (take < 8) take = 8;
            if (take > 64) take = 64;

            var eventsFromDb = await _eventService.GetLatestEventsAsync(skip, take);
            var totalEventsCount = await _eventService.GetEventsCount();
            var homeEventCards = eventsFromDb.Select(ToCard).ToList();

            var pageModel = new HomeIndexViewModel
            {
                Events = homeEventCards,
                Skip = skip,
                Take = take,
                TotalCount = totalEventsCount
            };

            return View(pageModel);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMore(int skip = 0, int take = 8)
        {
            if (skip < 0) skip = 0;
            if (take < 1) take = 8;
            if (take > 64) take = 64;

            var eventsFromDb = await _eventService.GetLatestEventsAsync(skip, take);
            var totalEventsCount = await _eventService.GetEventsCount();
            var eventCards = eventsFromDb.Select(ToCard).ToList();

            var chunkModel = new EventsChunkViewModel
            {
                Events = eventCards,
                NextSkip = skip + take,
                HasMore = (skip + take) < totalEventsCount
            };

            return PartialView("EventsChunk", chunkModel);
        }

        private static HomeEventCardViewModel ToCard(EventDto eventData)
        {
            return new HomeEventCardViewModel
            {
                Id = eventData.Id,
                Title = eventData.Title,
                Description = eventData.Description,
                Location = eventData.Location,
                StartAt = eventData.StartAt,
                CategoryName = eventData.CategoryName,
                ImageUrl = eventData.ImageUrl
            };
        }
    }
}
