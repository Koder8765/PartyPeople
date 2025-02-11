namespace Website.Models;

public class EmployeeListViewModel
{
    public required IReadOnlyCollection<Employee> Employees { get; init; }
}
