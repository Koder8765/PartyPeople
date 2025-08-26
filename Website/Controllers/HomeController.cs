using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Website.Models;
using Website.Persistence;
using Website.Repositories;

namespace Website.Controllers;

public class HomeController : Controller
{
    private readonly DbContext _dbContext;
    private readonly ILogger<HomeController> _logger;
    private readonly EventAttendeeRepository _eventAttendeeRepository;
    private readonly EmployeeRepository _employeeRepository;


    public HomeController(DbContext dbContext,
        ILogger<HomeController> logger,
        EventAttendeeRepository eventAttendeeRepository,
        EmployeeRepository employeeRepository)
    {
        _dbContext = dbContext;
        _logger = logger;
        _eventAttendeeRepository = eventAttendeeRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var events = await _dbContext.Events.GetAllAsync(includeHistoricEvents: false, cancellationToken);
        var upcomingEvents = events.Where(@event => @event.StartDateTime >= DateTime.Now && @event.StartDateTime <= DateTime.Now.AddDays(7));

        // Get top 5 attendees using the repository method
        var topEmployees = await _eventAttendeeRepository.GetTopAttendeesAsync(cancellationToken);

        // Prepare the model for the view
        var model = new HomeViewModel
        {
            Events = upcomingEvents,
            TopAttendees = topEmployees
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
