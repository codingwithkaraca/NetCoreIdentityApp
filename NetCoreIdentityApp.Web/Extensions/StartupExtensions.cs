using NetCoreIdentityApp.DataAccess.Concrete.EntityFramework;
using NetCoreIdentityApp.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using NetCoreIdentityApp.Web.CustomValidations;
using NetCoreIdentityApp.Web.Localizations;

namespace NetCoreIdentityApp.Web.Extensions;

public static class StartupExtensions
{
    public static void AddIdentityWithExtension(this IServiceCollection services)
    {
        // üretilecek token ömrü
        services.Configure<DataProtectionTokenProviderOptions>(opt =>
        {
            opt.TokenLifespan = TimeSpan.FromHours(2);
        });
        
        services.AddIdentity<User, UserRole>(options => 
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz0123456789-._";
        
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.MaxFailedAccessAttempts = 3;
            })
            .AddPasswordValidator<PasswordValidator>()
            .AddUserValidator<UserValidator>()
            .AddErrorDescriber<LocalizationIdentityErrorDescriber>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<NetCoreIdentityAppContext>();

    }
}