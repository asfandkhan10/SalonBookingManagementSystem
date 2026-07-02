using FluentValidation;
using SalonBookingSystem.Application.DTOs.User;

namespace SalonBookingSystem.Application.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(role => role == "Administrator" || role == "Receptionist" || role == "Customer")
            .WithMessage("Role must be Administrator, Receptionist, or Customer");
    }
}
