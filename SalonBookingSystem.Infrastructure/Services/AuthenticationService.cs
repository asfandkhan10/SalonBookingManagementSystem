using Microsoft.AspNetCore.Identity;
using SalonBookingSystem.Application.DTOs.Authentication;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Domain.Entities;
using SalonBookingSystem.Persistence.Context;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ApplicationUser = SalonBookingSystem.Persistence.Context.ApplicationUser;

namespace SalonBookingSystem.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ICustomerRepository customerRepository,
        ICurrentUserService currentUserService,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator,
        ILogger<AuthenticationService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _customerRepository = customerRepository;
        _currentUserService = currentUserService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _logger = logger;
    }

    public async Task<AuthenticationResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_registerValidator, request, cancellationToken);

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new AuthenticationResponse
            {
                Success = false,
                Message = "Email is already registered"
            };
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
            return new AuthenticationResponse
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            };
        }

        // Add Customer role
        await _userManager.AddToRoleAsync(user, "Customer");

        // Create Customer record linked to ApplicationUser
        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            ApplicationUserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.GetUserId(),
            IsDeleted = false
        };

        await _customerRepository.CreateAsync(customer, cancellationToken);

        _logger.LogInformation("User registered successfully with email: {Email}", request.Email);

        return new AuthenticationResponse
        {
            Success = true,
            Message = "Registration successful",
            CustomerId = customer.Id
        };
    }

    public async Task<AuthenticationResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_loginValidator, request, cancellationToken);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new AuthenticationResponse
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            request.Password,
            isPersistent: false,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return new AuthenticationResponse
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        // Get customer ID
        var customer = await _customerRepository.GetByApplicationUserIdAsync(user.Id, cancellationToken);

        // Add CustomerId claim to the user
        if (customer != null)
        {
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim("CustomerId", customer.Id.ToString())
            };
            await _userManager.AddClaimsAsync(user, claims);
        }

        _logger.LogInformation("User logged in successfully with email: {Email}", request.Email);

        return new AuthenticationResponse
        {
            Success = true,
            Message = "Login successful",
            CustomerId = customer?.Id
        };
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
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
