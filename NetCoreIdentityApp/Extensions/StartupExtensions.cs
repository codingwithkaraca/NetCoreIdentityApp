using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using NetCoreIdentityApp.CustomValidations;
using NetCoreIdentityApp.Localizations;

namespace NetCoreIdentityApp.Extensions;

public static class StartupExtensions
{
    public static void AddIdentityWithExtension(this IServiceCollection services)
    {
        services.AddIdentity<User, UserRole>(options => 
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz0123456789-._";
        
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;

            })
            .AddPasswordValidator<PasswordValidator>()
            .AddUserValidator<UserValidator>()
            .AddErrorDescriber<LocalizationIdentityErrorDescriber>()
            .AddEntityFrameworkStores<NetCoreIdentityAppContext>();

    }
}