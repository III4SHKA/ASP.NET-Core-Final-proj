namespace FinalProject.ViewModels;

public class EventsChunkViewModel
{
    public List<HomeEventCardViewModel> Events { get; set; } = new();
    public int NextSkip { get; set; }
    public bool HasMore { get; set; }
}
