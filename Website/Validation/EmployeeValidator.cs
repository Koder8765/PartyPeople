using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using Website.Models;

namespace Website.Validation;

/// <summary>
/// The validator for <see cref="Employee"/> objects.
/// </summary>
public class EmployeeValidator : AbstractValidator<Employee>
{
    /// <summary>
    /// Instantiates a new instance of the validator.
    /// </summary>
    public EmployeeValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .NotNull()
            .MaximumLength(255);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .NotNull()
            .MaximumLength(255);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .NotNull();
        
        RuleFor(x => x.DateOfBirth)
            .LessThan(DateOnly.FromDateTime(DateTime.Now))
            .WithMessage($"The Date of Birth must be less than '{DateOnly.FromDateTime(DateTime.Now):dd/MM/yyyy}'.");

        RuleFor(x => x.FavouriteDrink)
            .MaximumLength(90)
            .When(x => !string.IsNullOrEmpty(x.FavouriteDrink))
            .WithMessage($"Leave this empty if you have no favourite drink, drink can be maximum 90 characters long.");

    }
}