using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.DTOs.Authentication;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Application.Validators;

namespace SalonBookingSystem.API.Controllers;

[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(
        IAuthenticationService authenticationService,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _authenticationService = authenticationService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest(ApiResponse<object>.Fail("Request body is required."));
        }

        var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Validation failed.", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
        }

        var result = await _authenticationService.RegisterAsync(request, cancellationToken);
        if (!result.Success)
        {
            return BadRequest(ApiResponse<AuthenticationResponse>.Fail(result.Message));
        }

        return Ok(ApiResponse<AuthenticationResponse>.Ok(result, "Registration successful"));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest(ApiResponse<object>.Fail("Request body is required."));
        }

        var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Validation failed.", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
        }

        var result = await _authenticationService.LoginAsync(request, cancellationToken);
        if (!result.Success)
        {
            return BadRequest(ApiResponse<AuthenticationResponse>.Fail(result.Message));
        }

        return Ok(ApiResponse<AuthenticationResponse>.Ok(result, "Login successful"));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await _authenticationService.LogoutAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(new object(), "Logout successful"));
    }
}
