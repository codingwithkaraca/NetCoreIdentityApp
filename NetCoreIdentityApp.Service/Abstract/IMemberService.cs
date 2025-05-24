using Microsoft.AspNetCore.Identity;
using NetCoreIdentityApp.Entities.ViewModels;

namespace NetCoreIdentityApp.Service.Abstract;

public interface IMemberService
{
    Task<UserVM> GetUserViewModelByUserNameAsync(string userName);
    Task LogOutAsync();
    Task<bool> CheckPasswordAsync(string userName, string password);
    Task<(bool, IEnumerable<IdentityError>?)> ChangePasswordAsync(string userName, string oldPassword,
        string newPassword);
}