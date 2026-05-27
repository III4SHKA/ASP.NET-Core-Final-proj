namespace FinalProject.ViewModels;

public class HomeIndexViewModel
{
    public List<HomeEventCardViewModel> Events { get; set; } = new();
    public int Skip { get; set; }
    public int Take { get; set; }
    public int TotalCount { get; set; }
    public int NextSkip => Skip + Take;
    public bool HasMore => NextSkip < TotalCount;
}
