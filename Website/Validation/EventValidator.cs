using FluentValidation;
using Website.Models;

namespace Website.Validation;

/// <summary>
/// The validator for <see cref="Event"/> objects.
/// </summary>
public class EventValidator : AbstractValidator<Event>
{
    /// <summary>
    /// Instantiates a new instance of the validator.
    /// </summary>
    public EventValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .NotNull()
            .MaximumLength(255);

        RuleFor(x => x.StartDateTime)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.StartDateTime)
            .GreaterThan(DateTime.Now)
            .WithMessage($"The Event start date must be in the future.");

        RuleFor(x => x.EndDateTime)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.EndDateTime)
            .GreaterThan(x => x.StartDateTime)
            .WithMessage($"The Event end date must be be after the start date.");

        RuleFor(x => x.MaximumCapacity)
            .GreaterThan(0)
            .When(x => x.MaximumCapacity is not null)
            .WithMessage($"The maximum capacity must be greater than 0. Leave this empty if there is no limit.");
    }
}

