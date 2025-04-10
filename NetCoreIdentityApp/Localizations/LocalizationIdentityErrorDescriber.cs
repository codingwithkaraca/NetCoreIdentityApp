using Microsoft.AspNetCore.Identity;

namespace NetCoreIdentityApp.Localizations;

public class LocalizationIdentityErrorDescriber : IdentityErrorDescriber
{
     public override IdentityError DuplicateUserName(string userName)
     {
          return new IdentityError()
          {
               Code = "DuplicateUserName", 
               Description = $"{userName} kullanıcı adı daha önce başka bir kullanıcı tarafından alınmıştır"
          };
          //return base.DuplicateUserName(userName);
     }

     public override IdentityError DuplicateEmail(string email)
     {
          return new IdentityError()
          {
               Code = "DuplicateEmail", 
               Description = $"{email} daha önce başka bir kullanıcı tarafından alınmıştır",
          };
          //return base.DuplicateEmail(email);
     }

     public override IdentityError PasswordTooShort(int length)
     {
          return new IdentityError()
          {
               Code = "PasswordTooShort", Description = "Şifre en az 6 karakter olmalıdır"
          };
          //return base.PasswordTooShort(length);
     }

     public override IdentityError PasswordRequiresNonAlphanumeric()
     {
          return new IdentityError()
          {
               Code = "PasswordRequiresNonAlphanumeric", Description = "Şifreniz en az bir alfanumerik karakter içermelidir"
          };
          //return base.PasswordRequiresNonAlphanumeric();
     }

     public override IdentityError PasswordRequiresDigit()
     {
          return new IdentityError()
          {
               Code = "PasswordRequiresDigit", Description = "Şifreniz en az bir rakam içermelidir"
          };
          //return base.PasswordRequiresDigit();
     }

     public override IdentityError PasswordRequiresUpper()
     {
          return new IdentityError()
          {
               Code = "PasswordRequiresUpper", Description = "Şifreniz en az bir büyük harf içermelidir"
          };
          //return base.PasswordRequiresUpper();
     }

     public override IdentityError PasswordRequiresLower()
     {
          return new IdentityError()
          {
               Code = "PasswordRequiresLower", Description = "Şifreniz en az bir küçük harf içermelidir"
          };
          //return base.PasswordRequiresLower();
     }
}