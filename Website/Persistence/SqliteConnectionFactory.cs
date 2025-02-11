using Microsoft.Data.Sqlite;
using System.Data;

namespace Website.Persistence;

public class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    /// <summary>
    /// Initialises a new instance of the <see cref="SqliteConnectionFactory"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string to connect to.</param>
    public SqliteConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Creates a new closed database connection.
    /// </summary>
    public IDbConnection CreateConnection() => new SqliteConnection(_connectionString);
}