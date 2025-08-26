# Party People

## Scenario

At Koderly, our employees attend a lot of events throughout the year, and we need an application to track which events our employees will be attending.
Koderly has started creating an application called PartyPeople. The application is buggy and unfinished, and we need your help to complete it.

### Built With

The PartyPeople application is an ASP.NET Core MVC application, written in C# and targets the .NET 8 Framework. The application uses SQLite for its database and includes the following technologies.

- C#
- .NET 8
- ASP.NET Core
- [Dapper](https://github.com/DapperLib/Dapper)
- [FluentValidation](https://docs.fluentvalidation.net/en/latest/)
- [Bootstrap](https://getbootstrap.com/docs/5.3/getting-started/introduction/)

## Getting Started

### Prerequisites

Development for the PartyPeople application relies on the below software:

- Visual Studio 2022 Community Edition, incl. ASP.NET and Web Development

### Build Steps

To build the PartyPeople application, you need to follow the steps below.

1. Clone the repository

   ```
   git clone https://github.com/Coder8765/PartyPeople.git
   ```

2. Open the PartyPeople.sln in Visual Studio

3. Build and Run the Website project in Visual Studio
x``
## Tasks

You only need to spend as much time on the tasks as you feel necessary to demonstrate your capabilities, and you don't need to complete every task.
We estimate that it could take between four and seven hours to complete all tasks if you choose to do so.

To complete the application, you need to carry out the tasks below.

### Task 1 - Done

The PartyPeople project does not currently build. Can you help figure out why, and resolve the build issues?

Installed a Nuget package for Dapper dependencies

### Task 2 - Done

A bug has been reported that updating events is not working as expected. Can you help by debugging the functionality and resolving the issue?

Debugging with browser console didn't yield anything obvious from the get go apart from 304 during any action taken,  nowever it was a red herring as
when I have created multiplies entries the browser gave an error and updated all of the Event descriptions. I have inspected all of the EventRepositories
and spotted that the update command (UpdateAsync) is not specifc enough:

                UPDATE  [Event]
                SET     [Description] = @Description,
                        [StartDateTime] = @StartDateTime,
                        [EndDateTime] = @EndDateTime,
                        [MaximumCapacity] = @MaximumCapacity;
                        WHERE   [E].[Id] = @Id;

                SELECT  [E].[Id],
                        [E].[Description],
                        [E].[StartDateTime],
                        [E].[EndDateTime],
                        [E].[MaximumCapacity]
                FROM    [Event] AS [E];
                WHERE   [E].[Id] = @Id;

Meaning this will Update all Event's during update action as it's not specifying specfically which Event to update (no Where clause that would
identify the unique part of the Event.)
The fix is simple, to update with a Where clause.

I have also added to see the update event adding Console.WriteLine($"Updating event with ID: {@event.Id}");


### Task 3 - Done

Koderly would like to track which employees are attending which events. Can you extend the PartyPeople application to add this functionality?

The plan was to make an additional page within the Even editor screen called "Manage Attendees" where a user can come in and tick boxes of all users in the system.

To implement it I have created an EventAttendee table to track which employees are attending which events combining both EventId and EmployeeId also knowns as Many-to-Many relationship data model.
Reverse engineering practice from two previous models, I have added CreateTableIfNotExistsAsync method to ensure the EventAttendee table is created automatically at startup:

        CREATE TABLE IF NOT EXISTS [EventAttendee] (
            [EventId] INTEGER NOT NULL,
            [EmployeeId] INTEGER NOT NULL,
            PRIMARY KEY (EventId, EmployeeId),
            FOREIGN KEY (EventId) REFERENCES Event(Id) ON DELETE CASCADE,
            FOREIGN KEY (EmployeeId) REFERENCES Employee(Id) ON DELETE CASCADE
        )

Created event attendee controller that allows for two methods:

- GET: /EventAttandee/Manage/5, allows to initialize the manage page with all of the employees
- POST: /EventAttendee/Manage/5, allows to make an update to an Event to add attendees

Repository wise, I have created a dedicated EventAttendeeRepository with the following methods:

- Add attendees (AddAttendeeAsync)
- Replace attendees (ReplaceAttendeesAsync)
- Retrieve all attendees for a given event (GetAttendeesForEventAsync)

Made appropriate references for EventAttendeeRepository in main DbContext file for full access across the app.

Put the link in Details toolbar to link to the appropriate page via
    <a class="btn btn-outline-secondary btn-sm m-1" role="button" asp-controller="EventAttendee" asp-action="Manage" asp-route-eventId="@Model.Id">Manage Attendees</a>


Additionally: I have added 2 more features - count of actual attendants and a warning when picking employees above maximum capacity.

Changes made:
For attende count:
- In EventAttendeeViewModel, ive added CurrentAttendeesCount  to look at a total count of selected Employees.
- In EventAttendees, I make the calculation via the following var currentAttendeesCount = attendees.Count;
- For it to be seen in UI I have decided to go with minimalism and made it as a number in bracked near Maximum capacity so there would not be addditional UI clutter by simply adding this <p>Current Attendees: @Model.CurrentAttendeesCount</p>

Maximum capacity warning:
- In EventAttendeeViewModel, I introduced a boolean property track that makes a simple check wherever the Max Capacity check was achieved or overpicked via MaxCapacityExceeded boolean.
- To actually check for MaximumCapacity, I compare the currently picked attendees over all number vs >= of Maximum capacity value (or if it even has a value)
- For UI, I have made a conditional logic to display warning (but still allowing the end user to pick more attendees) if the maximum is exceeded by doing the following 
 @if (Model.MaxCapacityExceeded)
{
    <div class="alert alert-warning mt-3">
        <strong>Warning:</strong> The number of selected attendees has reached or exceeded the event's maximum capacity of @Model.Event.MaximumCapacity.
    </div>
}



### Task 4 - Done

Koderly would like to track which drinks should be ordered for employees. Can you extend the functionality to allow each employee to optionally specify a ‘Favourite Drink’?

Added a new entry in Employee class, a nullable string called FavouriteDrink, made a new validator entry so the FavouriteDrink field could be 90 characters max (cause who needs to have a triple digit character long drink) even then this might not be safe from sql
injection in the future.
Noww to actually get to the user page, i would need to either drop all the data within the table and re-create it within the db itself or Alter the table for the existing db. I have decided to go the Alter Table route (I've used DBeaver for this one) with
the following function:

ALTER TABLE Employee ADD COLUMN [FavouriteDrink] NVARCHAR(90) NULL

Obviously, before that i went in and added all UI elemnts for this to work on UI within the cshtml files.

### Task 5 - Done

Koderly would like to track the five most social employees (i.e. employees who have attended the most events). Can you add a widget to the Home screen to display this information?

The idea is to replicate the script responsible for "Event Upcoming in 7 Days" in the main index page.

I have created a new addition to EventAttendeeRepository.cs that checks for top 5 EventCount when doing a join between EventAttendee and Employee and Grouping them based on Employees Id which logically checks the amount of time Employee Id is referenced.
Obviously it's limited to 5 and picks only 3 headers so the query would be as optimised as possible:

            SELECT E.Id, E.FirstName, E.LastName, COUNT(EA.EventId) AS EventCount
            FROM Employee E
            JOIN EventAttendee EA ON E.Id = EA.EmployeeId
            GROUP BY E.Id
            ORDER BY EventCount DESC
            LIMIT 5;

After this, I updated the Home Controller to call this method and pass the data to the Home page for display.

As a last note I struggled slightly to add dependecies. For context, the repositories (EmployeeRepository, EventRepository, EventAttendeeRepository) depend on IDbConnectionProvider for database connections.
I realized I forgot to register IDbConnectionProvider in the DI container, which caused the runtime error:

Unable to resolve service for type 'IDbConnectionProvider' while attempting to activate 'EmployeeRepository'



### Task 6

Koderly would like to track upcoming events which have no attendees registered. Can you add a widget to the Home screen to display this information?

## Submission

Please commit your work for review by **11 am** on **Thursday, 28 August 2025**, by completing the steps below.

1. Fork the PartyPeople project
2. Create a Feature Branch (`git checkout -b feature/{featureName}`)
3. Commit your changes (`git commit -m 'My Commit Note'`)
4. Push to your branch (`git push origin feature/{featureName}`)
5. Open a Pull Request

## Contact

If you need help with a particular task, or cannot proceed for any reason, please don’t hesitate to contact us by email at [recruitment@koder.ly](mailto:recruitment@koder.ly).
