namespace Website.Models
{
    public class EventAttendeeViewModel
    {
        public required Event Event { get; init; }
        public required IReadOnlyCollection<Employee> AllEmployees {  get; init; }

        public required List<int> SelectedEmployeeIds {  get; init; }

        public int AttendeeCount { get; set; }
        public bool MaxCapacityExceeded { get; set; }
    }
}
