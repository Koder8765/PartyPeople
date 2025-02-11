using System.Data;

namespace Website.Persistence;

/// <summary>
/// Interface to an object that provides a database connection.
/// </summary>
public interface IDbConnectionProvider
{
    /// <summary>
    /// Gets a database connection.
    /// </summary>
    IDbConnection GetConnection();
}