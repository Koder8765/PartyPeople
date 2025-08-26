using Dapper;
using NuGet.Protocol.Plugins;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Transactions;
using Website.Models;
using Website.Persistence;

namespace Website.Repositories
{
    public class EventAttendeeRepository : RepositoryBase
    {
        public EventAttendeeRepository(IDbConnectionProvider connectionProvider)
            : base(connectionProvider)
        {
        }

        /// <summary>
        /// Creates the event attandee table, if it doesn't already exist.
        /// </summary>
        /// <param name="cancellationToken">A token which can be used to cancel asynchronous operations.</param>
        /// <returns>An awaitable task</returns>
        public async Task CreateTableIfNotExistsAsync(CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(
                @"
        CREATE TABLE IF NOT EXISTS [EventAttendee] (
            [EventId] INTEGER NOT NULL,
            [EmployeeId] INTEGER NOT NULL,
            PRIMARY KEY (EventId, EmployeeId),
            FOREIGN KEY (EventId) REFERENCES Event(Id) ON DELETE CASCADE,
            FOREIGN KEY (EmployeeId) REFERENCES Employee(Id) ON DELETE CASCADE
        );
        ",
                commandType: CommandType.Text,
                cancellationToken: cancellationToken
            );

            await Connection.ExecuteAsync(command);
        }

        /// <summary>
        /// Adds an attendee to Event but ignores the function if Employee is already tied to that event
        /// </summary>
        public async Task AddAttendeeAsync(int eventId, int employeeId, CancellationToken cancellationToken = default)
        {
            var command = new CommandDefinition(
                                @"
                    INSERT OR IGNORE INTO [EventAttendee] ([EventId], [EmployeeId])
                    VALUES (@EventId, @EmployeeId);
                ",

                new { EventId = eventId, EmployeeId = employeeId },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken
            );

            await Connection.ExecuteAsync(command);
        }

        /// <summary>
        /// Retrieve possible attendees.
        /// </summary>
        public async Task<IReadOnlyCollection<Employee>> GetAttendeesForEventAsync(int eventId, CancellationToken cancellationToken = default)
        {
            var command = new CommandDefinition(
                @"
                    SELECT  E.Id, E.FirstName, E.LastName, E.DateOfBirth, E.FavouriteDrink
                    FROM    Employee E
                    JOIN    EventAttendee EA ON E.Id = EA.EmployeeId
                    WHERE   EA.EventId = @EventId;
                ",
                new { EventId = eventId },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken
            );

            var attendees = await Connection.QueryAsync<Employee>(command);
            return attendees.ToArray();
        }
        /// <summary>
        /// Switch attendees.
        /// </summary>
        public async Task ReplaceAttendeesAsync(int eventId, List<int> employeeIds, CancellationToken cancellationToken = default)
        {
            var connection = Connection;

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await ((DbConnection)connection).OpenAsync(cancellationToken);//abandons before connection timeout happens
            }

            using var transaction = connection.BeginTransaction();

            ///<summary>
            /// Remove existing attendees
            /// </summary>
            await connection.ExecuteAsync(
                "DELETE FROM EventAttendee WHERE EventId = @EventId;",
                new { EventId = eventId },
                transaction
            );
            /// <summary>
            /// Insert new ones
            /// </summary>
            foreach (var employeeId in employeeIds)
            {
                await connection.ExecuteAsync(
                    "INSERT INTO EventAttendee (EventId, EmployeeId) VALUES (@EventId, @EmployeeId);",
                    new { EventId = eventId, EmployeeId = employeeId },
                    transaction
                );
            }

            transaction.Commit();
        }

        public async Task<IReadOnlyList<Employee>> GetTopAttendeesAsync(CancellationToken cancellationToken = default)
        {
            var command = new CommandDefinition(
                @"
            SELECT E.Id, E.FirstName, E.LastName, COUNT(EA.EventId) AS EventCount
            FROM Employee E
            JOIN EventAttendee EA ON E.Id = EA.EmployeeId
            GROUP BY E.Id
            ORDER BY EventCount DESC
            LIMIT 5;
        ",
                commandType: CommandType.Text,
                cancellationToken: cancellationToken
            );

            var topAttendees = await Connection.QueryAsync<Employee>(command);
            return topAttendees.ToList();
        }
    }
}