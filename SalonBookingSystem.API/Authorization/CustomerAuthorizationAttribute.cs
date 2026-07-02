using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SalonBookingSystem.API.Authorization;

public class CustomerAuthorizationAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var customerIdClaim = user.FindFirst("CustomerId");
        if (customerIdClaim == null || !int.TryParse(customerIdClaim.Value, out var customerId))
        {
            context.Result = new ForbidResult();
            return;
        }

        // Add customer ID to HttpContext items for use in controllers
        context.HttpContext.Items["CustomerId"] = customerId;
    }
}
