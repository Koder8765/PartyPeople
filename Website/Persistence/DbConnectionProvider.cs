using System.Data;

namespace Website.Persistence;

/// <summary>
/// A database connection provider that returns a new closed connection each time.
/// </summary>
public class DbConnectionProvider : IDbConnectionProvider
{
    private readonly IDbConnectionFactory _factory;

    /// <summary>
    /// Initialises a new instance of the <see cref="DbConnectionProvider"/> class.
    /// </summary>
    /// <param name="factory">The connection factory to get the closed connections from.</param>
    public DbConnectionProvider(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Gets a database connection.
    /// </summary>
    public IDbConnection GetConnection() => _factory.CreateConnection();
}
