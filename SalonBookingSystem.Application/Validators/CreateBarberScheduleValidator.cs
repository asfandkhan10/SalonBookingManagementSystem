using FluentValidation;
using SalonBookingSystem.Application.DTOs.BarberSchedule;

namespace SalonBookingSystem.Application.Validators;

public class CreateBarberScheduleValidator : AbstractValidator<CreateBarberScheduleRequest>
{
    public CreateBarberScheduleValidator()
    {
        RuleFor(x => x.BarberId)
            .GreaterThan(0);

        RuleFor(x => x.DayOfWeek)
            .IsInEnum();

        RuleFor(x => x.StartTime)
            .NotEmpty();

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .GreaterThan(x => x.StartTime)
            .WithMessage("EndTime must be greater than StartTime");
    }
}
