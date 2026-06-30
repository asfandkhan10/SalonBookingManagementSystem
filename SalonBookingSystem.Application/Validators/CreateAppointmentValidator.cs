using FluentValidation;
using SalonBookingSystem.Application.DTOs.Appointment;

namespace SalonBookingSystem.Application.Validators;

public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentRequest>
{
    public CreateAppointmentValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0);

        RuleFor(x => x.BarberId)
            .GreaterThan(0);

        RuleFor(x => x.AppointmentDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateTime.Today);

        RuleFor(x => x.StartTime)
            .NotEmpty();

        RuleFor(x => x.ServiceIds)
            .NotEmpty()
            .WithMessage("At least one service must be selected");
    }
}
