using SalonBookingSystem.Application.DTOs.User;

namespace SalonBookingSystem.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<List<UserResponse>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<UserResponse?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
}
