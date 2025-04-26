using Entities.Concrete;
using Microsoft.AspNetCore.Identity;

namespace NetCoreIdentityApp.CustomValidations;

public class UserValidator : IUserValidator<User>
{
    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
    {
        List<IdentityError> errors = new List<IdentityError>();

        bool isDigit = int.TryParse(user.UserName![0].ToString(), out _);
        if (isDigit)
        {
            errors.Add(new IdentityError() {Code = "UsernameContainFirstLetterDigit", Description = "Kullanıcı adı ilk karakteri sayısal bir karakter içeremez"});
        }

        if (errors.Any())
        {
            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
        }

        return Task.FromResult(IdentityResult.Success);
    }
}