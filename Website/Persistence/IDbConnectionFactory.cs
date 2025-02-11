using System.Data;

namespace Website.Persistence;

/// <summary>
/// Interface to an object that creates database connections.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates a new closed database connection.
    /// </summary>
    IDbConnection CreateConnection();
}
