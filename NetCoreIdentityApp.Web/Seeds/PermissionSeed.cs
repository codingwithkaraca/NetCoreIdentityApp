using System.Security.Claims;
using NetCoreIdentityApp.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using NetCoreIdentityApp.Core.PermissionsRoot;

namespace NetCoreIdentityApp.Web.Seeds;

public class PermissionSeed
{
    public static async Task Seed(RoleManager<UserRole> roleManager)
    {
        var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");
        var hasAdvancedRole = await roleManager.RoleExistsAsync("AdvancedRole");
        var hasAdminRole = await roleManager.RoleExistsAsync("AdminRole");

        if (!hasBasicRole)
        {
            await roleManager.CreateAsync(new UserRole(){Name = "BasicRole"});
            var basicRole = (await roleManager.FindByNameAsync("BasicRole"))!;

            await AddReadPermission(basicRole, roleManager);
        }
        
        if (!hasAdvancedRole)
        {
            await roleManager.CreateAsync(new UserRole(){Name = "AdvancedRole"});
            var basicRole = (await roleManager.FindByNameAsync("AdvancedRole"))!;

            await AddReadPermission(basicRole, roleManager);
            await AddCreateAndUpdatePermission(basicRole, roleManager);
        }
        
        if (!hasAdminRole)
        {
            await roleManager.CreateAsync(new UserRole(){Name = "AdminRole"});
            var basicRole = (await roleManager.FindByNameAsync("AdminRole"))!;

            await AddReadPermission(basicRole, roleManager);
            await AddCreateAndUpdatePermission(basicRole, roleManager);
            await AddDeletePermission(basicRole, roleManager);
        }
        
    }

    public static async Task AddReadPermission(UserRole role, RoleManager<UserRole> roleManager)
    {
        await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Read));

        await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Read));

        await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Read));
    }
    
    public static async Task AddCreateAndUpdatePermission(UserRole role, RoleManager<UserRole> roleManager)
    {
        await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Create));
        await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Create));
        await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Create));

        await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Update));
        await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Update));
        await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Update));
    }
    
    public static async Task AddDeletePermission(UserRole role, RoleManager<UserRole> roleManager)
    {
        await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Delete));
        await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Delete));
        await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Delete));
    }
}