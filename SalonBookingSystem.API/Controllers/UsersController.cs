using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SalonBookingSystem.API.Authorization;
using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.DTOs.User;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Application.Validators;

namespace SalonBookingSystem.API.Controllers;

[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserRequest> _createValidator;

    public UsersController(IUserService userService, IValidator<CreateUserRequest> createValidator)
    {
        _userService = userService;
        _createValidator = createValidator;
    }

    [HttpGet]
    [AdministratorAuthorization]
    public async Task<ActionResult<ApiResponse<List<UserResponse>>>> GetUsers(CancellationToken cancellationToken = default)
    {
        var users = await _userService.GetUsersAsync(cancellationToken);
        return Ok(ApiResponse<List<UserResponse>>.Ok(users));
    }

    [HttpGet("{id}")]
    [AdministratorAuthorization]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetUserById(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetUserByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return NotFound(ApiResponse<UserResponse>.Fail($"User with id {id} was not found."));
        }

        return Ok(ApiResponse<UserResponse>.Ok(user));
    }

    [HttpPost]
    [AdministratorAuthorization]
    public async Task<ActionResult<ApiResponse<UserResponse>>> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return BadRequest(ApiResponse<object>.Fail("Request body is required."));
        }

        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Validation failed.", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
        }

        var user = await _userService.CreateUserAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, ApiResponse<UserResponse>.Ok(user, "User created successfully."));
    }

    [HttpDelete("{id}")]
    [AdministratorAuthorization]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(string id, CancellationToken cancellationToken = default)
    {
        var deleted = await _userService.DeleteUserAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail($"User with id {id} was not found."));
        }

        return Ok(ApiResponse<object>.Ok(null!, "User deleted successfully."));
    }

    [HttpPost("{id}/assign-role")]
    [AdministratorAuthorization]
    public async Task<ActionResult<ApiResponse<object>>> AssignRole(string id, [FromBody] AssignRoleRequest request, CancellationToken cancellationToken = default)
    {
        var assigned = await _userService.AssignRoleAsync(id, request.Role, cancellationToken);
        if (!assigned)
        {
            return NotFound(ApiResponse<object>.Fail($"User with id {id} was not found."));
        }

        return Ok(ApiResponse<object>.Ok(null!, "Role assigned successfully."));
    }
}
