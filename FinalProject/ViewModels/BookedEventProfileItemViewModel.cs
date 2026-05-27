namespace FinalProject.ViewModels;

public class BookedEventProfileItemViewModel
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime BookedAt { get; set; }
}
