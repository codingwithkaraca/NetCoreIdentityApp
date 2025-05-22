using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace NetCoreIdentityApp.Web.Requirements;

public class ViolenceRequirement : IAuthorizationRequirement
{
    public int ThresholdAge { get; set; }

}

public class ViolenceRequirementHandler : AuthorizationHandler<ViolenceRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolenceRequirement requirement)
    {
        if (!context.User.HasClaim(x => x.Type == "birthdate"))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        Claim userBirthDateClaim = context.User.FindFirst("birthdate")!;

        DateTime today = DateTime.Now;
        DateTime birthDate =  Convert.ToDateTime(userBirthDateClaim.Value);
        int age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age)) age--;
        
        if (requirement.ThresholdAge > age)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}