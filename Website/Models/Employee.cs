using Microsoft.CodeAnalysis;
using System.ComponentModel;

namespace Website.Models;

/// <summary>
/// The employee model.
/// </summary>
public class Employee
{
    /// <summary>
    /// The unique identifier for this employee model.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// The first name for this employee model.
    /// </summary>
    [DisplayName("First Name")]
    public required string FirstName { get; init; }

    /// <summary>
    /// The last name for this employee model.
    /// </summary>
    [DisplayName("Last Name")]
    public required string LastName { get; init; }

    /// <summary>
    /// The date of birth for this employee model.
    /// </summary>
    [DisplayName("Date of Birth")]
    public required DateOnly DateOfBirth { get; init; }

    /// <summary>
    /// Favourite drink of the employee so event organisers know what to serve.
    /// </summary>
    /// <remarks>
    /// When <see langword="null"/>, that means the employee does not have a specific drink in mind
    /// </remarks>
    [DisplayName("Favourite Drink")]
    public string? FavouriteDrink { get; init; }    
}