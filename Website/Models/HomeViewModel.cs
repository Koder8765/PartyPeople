namespace Website.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Event>?   Events { get; set; }
        public IReadOnlyList<Employee>? TopAttendees { get; set; }
    }
}
