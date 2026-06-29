using FluentValidation;
using SalonBookingSystem.Application.DTOs.Barber;

namespace SalonBookingSystem.Application.Validators;

public class UpdateBarberValidator : AbstractValidator<UpdateBarberRequest>
{
    public UpdateBarberValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20);
    }
}
