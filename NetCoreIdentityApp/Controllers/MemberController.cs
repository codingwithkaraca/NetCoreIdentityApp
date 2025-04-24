using Entities.Concrete;
using Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        // bu çıkış yap butonu çıkış yaptıktan sonra bana hangi url'e gitme avantajını sağlıyor
        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
        }

    }
}
