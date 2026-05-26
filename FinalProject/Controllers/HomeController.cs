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

        public async Task<IActionResult> Index()
        {
            var latestEvents = await _eventService.GetLatestEventsAsync();
            var homeEventCards = latestEvents.Select(MapToCardViewModel).ToList();
            return View(homeEventCards);
        }

        private static HomeEventCardViewModel MapToCardViewModel(EventDto eventData)
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
