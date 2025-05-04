using Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreIdentityApp.Areas.Admin.Models;
using NetCoreIdentityApp.Extensions;

namespace NetCoreIdentityApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRole> _roleManager;

        public RolesController(UserManager<User> userManager, RoleManager<UserRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        
        public async Task<ActionResult> Index()
        {
            var roleList = await _roleManager.Roles.Select(x => new RoleVM()
            {
                Id = x.Id,
                Name = x.Name!,
            }).ToListAsync();
            return View(roleList);
        }
        
        public ActionResult RoleCreate()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<ActionResult> RoleCreate(RoleCreateVM requestModel)
        {
            var result = await _roleManager.CreateAsync(new UserRole { Name = requestModel.Name });

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }
            
            TempData["SuccessMessage"] = "Rol başarıyla kayıt edildi";
            return RedirectToAction(nameof(RolesController.Index));
        }
        
        
        
    }
}
