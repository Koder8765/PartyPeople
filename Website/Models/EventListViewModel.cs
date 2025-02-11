namespace Website.Models;

public class EventListViewModel
{
    public required bool IsShowingHistoricEvents { get; init; }
    public required IReadOnlyCollection<Event> Events { get; init; }
}
