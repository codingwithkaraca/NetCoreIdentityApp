using Entities.Concrete;
using Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreIdentityApp.Extensions;

namespace NetCoreIdentityApp.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        public MemberController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            var userViewModel = new UserVM
            {
                UserName = currentUser!.UserName,
                Email = currentUser!.Email,
                PhoneNumber = currentUser!.PhoneNumber,
            };
            
            return View(userViewModel);
        }

        public IActionResult PasswordChange()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeVM requestModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
            var chechPassword = await _userManager.CheckPasswordAsync(currentUser,requestModel.PasswordOld);
            if (!chechPassword)
            {
                ModelState.AddModelError(String.Empty, "Eski şifreniz geçersizdir");
                return View();
            }

            var resultChangePassword =
               await _userManager.ChangePasswordAsync(currentUser, requestModel.PasswordOld, requestModel.PasswordNew);

            if (!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors.Select(x => x.Description).ToList());
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser,requestModel.PasswordNew,true,false);
            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirilmiştir";
            return View();
        }

        public async Task<IActionResult> UserEdit()
        {
            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var userEditModel = new UserEditVM
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender
            };
            
            return View(userEditModel);
        }
        
        [HttpPost]
        public IActionResult UserEdit(UserEditVM requestModel)
        {
            
            return View();
        }
        

        // bu çıkış yap butonu çıkış yaptıktan sonra bana hangi url'e gitme avantajını sağlıyor
        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
        }

    }
}
