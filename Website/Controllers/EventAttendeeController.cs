using Microsoft.AspNetCore.Mvc;
using Website.Models;
using Website.Persistence;

namespace Website.Controllers
{
    public class EventAttendeeController : Controller
    {
        private readonly DbContext _dbContext;
        private readonly ILogger<EventAttendeeController> _logger;

        public EventAttendeeController(DbContext dbContext, ILogger<EventAttendeeController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // GET: /EventAttandee/Manage/5
        public async Task<IActionResult> Manage (int eventId, CancellationToken cancellationToken = default)
        {
            var @event = await _dbContext.Events.GetByIdAsync(eventId, cancellationToken);
            if (@event is null) return NotFound();

            var allEmployees = await _dbContext.Employees.GetAllAsync(cancellationToken);
            var attendees = await _dbContext.EventAttendees.GetAttendeesForEventAsync(eventId, cancellationToken);

            var selectedEmployeeIds = attendees.Select(e => e.Id).ToList();

            //checks if current number > max capacity of attendees
            var selectedEmployeeCount = selectedEmployeeIds.Count;
            var maxCapacityExceeded = @event.MaximumCapacity.HasValue && selectedEmployeeCount > @event.MaximumCapacity;


            // adding employees
            var model = new EventAttendeeViewModel
            {
                Event = @event,
                AllEmployees = allEmployees,
                SelectedEmployeeIds = selectedEmployeeIds,
                MaxCapacityExceeded = maxCapacityExceeded
            };

            return View(model);
        }

        // POST: /EventAttendee/Manage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(int eventId, List<int> selectedEmployeeIds, CancellationToken cancellationToken = default)
        {
            var @event = await _dbContext.Events.GetByIdAsync(eventId, cancellationToken);
            if (@event is null)
                return NotFound();

            await _dbContext.EventAttendees.ReplaceAttendeesAsync(eventId, selectedEmployeeIds, cancellationToken);

            return RedirectToAction("Details", "Event", new { id = eventId });
        }
    }
}