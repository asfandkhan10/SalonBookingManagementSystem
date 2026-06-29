using FluentValidation;
using SalonBookingSystem.Application.DTOs.Service;

namespace SalonBookingSystem.Application.Validators;

public class UpdateServiceValidator : AbstractValidator<UpdateServiceRequest>
{
    public UpdateServiceValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.Price)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.DurationMinutes)
            .NotEmpty()
            .GreaterThan(0);
    }
}
