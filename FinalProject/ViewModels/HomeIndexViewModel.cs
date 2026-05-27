namespace FinalProject.ViewModels;

public class HomeIndexViewModel
{
    public List<HomeEventCardViewModel> Events { get; set; } = new();
    public List<UpcomingEventViewModel> UpcomingEvents { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public string? SelectedCategory { get; set; }
    public string SearchQuery { get; set; } = string.Empty;
    public int Skip { get; set; }
    public int Take { get; set; }
    public int TotalCount { get; set; }
    public int NextSkip => Skip + Take;
    public bool HasMore => NextSkip < TotalCount;
}
