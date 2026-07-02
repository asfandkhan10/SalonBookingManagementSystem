using FluentValidation;
using SalonBookingSystem.Application.DTOs.Appointment;

namespace SalonBookingSystem.Application.Validators;

public class RescheduleAppointmentValidator : AbstractValidator<RescheduleAppointmentRequest>
{
    public RescheduleAppointmentValidator()
    {
        RuleFor(x => x.NewAppointmentDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Appointment date must be today or in the future");

        RuleFor(x => x.NewStartTime)
            .NotEmpty()
            .WithMessage("Start time is required");
    }
}
