using FluentValidation;
using SalonBookingSystem.Application.DTOs.Appointment;

namespace SalonBookingSystem.Application.Validators;

public class UpdateAppointmentValidator : AbstractValidator<UpdateAppointmentRequest>
{
    public UpdateAppointmentValidator()
    {
        RuleFor(x => x.AppointmentDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateTime.Today);

        RuleFor(x => x.StartTime)
            .NotEmpty();

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.ServiceIds)
            .NotEmpty()
            .WithMessage("At least one service must be selected");
    }
}
