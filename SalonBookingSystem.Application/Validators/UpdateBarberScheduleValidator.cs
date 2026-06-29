using FluentValidation;
using SalonBookingSystem.Application.DTOs.BarberSchedule;

namespace SalonBookingSystem.Application.Validators;

public class UpdateBarberScheduleValidator : AbstractValidator<UpdateBarberScheduleRequest>
{
    public UpdateBarberScheduleValidator()
    {
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
