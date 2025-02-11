using System.Data;
using Website.Persistence;

namespace Website.Repositories;

/// <summary>
/// Base class for repositories that access a database.
/// </summary>
public abstract class RepositoryBase
{
    private readonly IDbConnectionProvider _connectionProvider;

    /// <summary>
    /// The connection the repository should use to access the database.
    /// </summary>
    /// <remarks>
    /// If the returned connection is closed, the caller should close it after using it.
    /// If the connection is open, it should be left open. Dapper will behave this way by default.
    /// </remarks>
    protected IDbConnection Connection => _connectionProvider.GetConnection();

    /// <summary>
    /// Initialises a new instance of the <see cref="RepositoryBase"/> class.
    /// </summary>
    /// <param name="connectionProvider">The connection provider to use.</param>
    protected RepositoryBase(IDbConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }
}