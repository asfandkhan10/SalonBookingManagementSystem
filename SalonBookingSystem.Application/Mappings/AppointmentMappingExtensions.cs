using SalonBookingSystem.Application.DTOs.Appointment;
using SalonBookingSystem.Application.DTOs.AppointmentService;
using SalonBookingSystem.Domain.Entities;

namespace SalonBookingSystem.Application.Mappings;

public static class AppointmentMappingExtensions
{
    public static AppointmentResponse ToResponse(this Appointment appointment)
    {
        return new AppointmentResponse
        {
            Id = appointment.Id,
            CustomerId = appointment.CustomerId,
            BarberId = appointment.BarberId,
            AppointmentDate = appointment.AppointmentDate,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            Status = appointment.Status,
            TotalAmount = appointment.TotalAmount,
            TotalDurationMinutes = appointment.TotalDurationMinutes,
            CreatedAt = appointment.CreatedAt,
            CreatedBy = appointment.CreatedBy,
            UpdatedAt = appointment.UpdatedAt,
            UpdatedBy = appointment.UpdatedBy
        };
    }

    public static Appointment ToEntity(this CreateAppointmentRequest request)
    {
        return new Appointment
        {
            CustomerId = request.CustomerId,
            BarberId = request.BarberId,
            AppointmentDate = request.AppointmentDate,
            StartTime = TimeSpan.Parse(request.StartTime),
            // Resolution 7: newly created appointments are Confirmed directly, Pending is not used
            Status = Domain.Enums.AppointmentStatus.Confirmed,
            TotalAmount = 0,
            TotalDurationMinutes = 0
        };
    }

    public static void ApplyUpdate(this Appointment appointment, UpdateAppointmentRequest request)
    {
        appointment.AppointmentDate = request.AppointmentDate;
        appointment.StartTime = TimeSpan.Parse(request.StartTime);
        appointment.Status = request.Status;
    }

    public static AppointmentServiceResponse ToResponse(this AppointmentService appointmentService)
    {
        return new AppointmentServiceResponse
        {
            Id = appointmentService.Id,
            AppointmentId = appointmentService.AppointmentId,
            ServiceId = appointmentService.ServiceId,
            Price = appointmentService.Price,
            DurationMinutes = appointmentService.DurationMinutes,
            CreatedAt = appointmentService.CreatedAt,
            CreatedBy = appointmentService.CreatedBy,
            UpdatedAt = appointmentService.UpdatedAt,
            UpdatedBy = appointmentService.UpdatedBy
        };
    }

    public static AppointmentService ToEntity(this CreateAppointmentServiceRequest request)
    {
        return new AppointmentService
        {
            AppointmentId = request.AppointmentId,
            ServiceId = request.ServiceId,
            Price = request.Price,
            DurationMinutes = request.DurationMinutes
        };
    }
}
