using Entities.Concrete;
using Microsoft.AspNetCore.Identity;

namespace NetCoreIdentityApp.CustomValidations;

public class PasswordValidator : IPasswordValidator<User>
{
    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string? password)
    {

        var errors = new List<IdentityError>();
        if (password.ToLower().Contains(user.UserName.ToLower()))
        {
            errors.Add(new IdentityError() {Code = "PasswordContainUserName", Description = "Şifre alanı kullanıcı adı içeremez"});
        }

        if (errors.Any())
        {
            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
        }

        return Task.FromResult(IdentityResult.Success);
    }
}