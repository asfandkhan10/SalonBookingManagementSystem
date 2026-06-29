using FluentValidation;
using SalonBookingSystem.Application.DTOs.Barber;

namespace SalonBookingSystem.Application.Validators;

public class CreateBarberValidator : AbstractValidator<CreateBarberRequest>
{
    public CreateBarberValidator()
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
