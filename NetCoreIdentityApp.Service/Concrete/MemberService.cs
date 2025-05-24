using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using NetCoreIdentityApp.Entities.Concrete;
using NetCoreIdentityApp.Entities.ViewModels;
using NetCoreIdentityApp.Service.Abstract;

namespace NetCoreIdentityApp.Service.Concrete;

public class MemberService : IMemberService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    public MemberService(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    public async Task<UserVM> GetUserViewModelByUserNameAsync(string userName)
    {
        var currentUser = (await _userManager.FindByNameAsync(userName))!;

        return new UserVM
        {
            UserName = currentUser.UserName,
            Email = currentUser.Email,
            PhoneNumber = currentUser.PhoneNumber,
            PictureUrl = currentUser.Picture,
        };
    }

    public async Task LogOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<bool> CheckPasswordAsync(string userName, string password)
    {
        var currentUser = await _userManager.FindByNameAsync(userName);
        return await _userManager.CheckPasswordAsync(currentUser, password);
    }

    public async Task<(bool, IEnumerable<IdentityError>?)> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
    {
        var currentUser = await _userManager.FindByNameAsync(userName);
        var resultChangePassword =
            await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);

        if (!resultChangePassword.Succeeded)
        {
            return (false,resultChangePassword.Errors);
        }
        
        await _userManager.UpdateSecurityStampAsync(currentUser);
        await _signInManager.SignOutAsync();
        await _signInManager.PasswordSignInAsync(currentUser, oldPassword, true, false);

        return (true, null);
    }

    public async Task<UserEditVM> GetUserEditViewModelByUserNameAsync(string userName)
    {
        var currentUser = await _userManager.FindByNameAsync(userName);
        return new UserEditVM
        {
            UserName = currentUser.UserName!,
            Email = currentUser.Email!,
            Phone = currentUser.PhoneNumber!,
            BirthDate = currentUser.BirthDate,
            City = currentUser.City,
            Gender = currentUser.Gender
        };
    }

    public List<ClaimVM> GetClaims(ClaimsPrincipal principal)
    {
        return principal.Claims.Select(x => new ClaimVM()
        {
            Issuer = x.Issuer,
            Type = x.Type,
            Value = x.Value,
        }).ToList();
        
    }
}