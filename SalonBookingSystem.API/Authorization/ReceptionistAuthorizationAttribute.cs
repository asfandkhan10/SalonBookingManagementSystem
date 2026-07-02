using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace SalonBookingSystem.API.Authorization;

public class ReceptionistAuthorizationAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!user.IsInRole("Receptionist") && !user.IsInRole("Administrator"))
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}
