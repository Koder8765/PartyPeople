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


### Task 3

Koderly would like to track which employees are attending which events. Can you extend the PartyPeople application to add this functionality?

### Task 4 - Done

Koderly would like to track which drinks should be ordered for employees. Can you extend the functionality to allow each employee to optionally specify a ‘Favourite Drink’?

Added a new entry in Employee class, a nullable string called FavouriteDrink, made a new validator entry so the FavouriteDrink field could be 90 characters max (cause who needs to have a triple digit character long drink) even then this might not be safe from sql
injection in the future.
Noww to actually get to the user page, i would need to either drop all the data within the table and re-create it within the db itself or Alter the table for the existing db. I have decided to go the Alter Table route (I've used DBeaver for this one) with
the following function:

ALTER TABLE Employee ADD COLUMN [FavouriteDrink] NVARCHAR(90) NULL

Obviously, before that i went in and added all UI elemnts for this to work on UI within the cshtml files.

### Task 5

Koderly would like to track the five most social employees (i.e. employees who have attended the most events). Can you add a widget to the Home screen to display this information?

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
