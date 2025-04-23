using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreIdentityApp.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        public MemberController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public IActionResult Index()
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
