using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SalonBookingSystem.Application.DTOs.User;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Persistence.Context;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ApplicationUser = SalonBookingSystem.Persistence.Context.ApplicationUser;

namespace SalonBookingSystem.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IValidator<CreateUserRequest> _validator;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IValidator<CreateUserRequest> validator,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_validator, request, cancellationToken);

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure(nameof(request.Email), "Email is already registered")
            });
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure(nameof(request.Password), string.Join(", ", result.Errors.Select(e => e.Description)))
            });
        }

        // Add role to user
        if (!await _roleManager.RoleExistsAsync(request.Role))
        {
            await _roleManager.CreateAsync(new IdentityRole(request.Role));
        }

        await _userManager.AddToRoleAsync(user, request.Role);

        _logger.LogInformation("User created successfully with email: {Email} and role: {Role}", request.Email, request.Role);

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email!,
            Role = request.Role
        };
    }

    public async Task<List<UserResponse>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users.ToListAsync(cancellationToken);
        var response = new List<UserResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            response.Add(new UserResponse
            {
                Id = user.Id,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "Customer"
            });
        }

        return response;
    }

    public async Task<UserResponse?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? "Customer"
        };
    }

    public async Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            _logger.LogInformation("User deleted successfully with id: {Id}", id);
            return true;
        }

        return false;
    }

    public async Task<bool> AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        // Ensure role exists
        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
        }

        // Remove existing roles
        var existingRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, existingRoles);

        // Add new role
        var result = await _userManager.AddToRoleAsync(user, role);
        if (result.Succeeded)
        {
            _logger.LogInformation("Role {Role} assigned to user {UserId}", role, userId);
            return true;
        }

        return false;
    }

    private async Task ValidateAsync<T>(IValidator<T> validator, T request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
    }
}
