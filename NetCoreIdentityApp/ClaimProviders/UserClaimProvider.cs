using System.Security.Claims;
using Entities.Concrete;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace NetCoreIdentityApp.ClaimProviders;

public class UserClaimProvider : IClaimsTransformation
{
    private readonly UserManager<User> _userManager;
    public UserClaimProvider(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identityUser = principal.Identity as ClaimsIdentity;
        var currentUser = await _userManager.FindByNameAsync(identityUser!.Name!);
        
        if (String.IsNullOrEmpty(currentUser!.City))
        {
            return principal;
        } 
        
        if (principal.HasClaim(x => x.Type != "city")) 
        {
            Claim cityClaim = new Claim("city", currentUser.City); 
            identityUser.AddClaim(cityClaim); 
        }
        
        return principal;
    }
}