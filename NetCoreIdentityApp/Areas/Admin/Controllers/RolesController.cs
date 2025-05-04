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

        public async Task<IActionResult> RoleUpdate(string id)
        {
            var currentUserRole = await _roleManager.FindByIdAsync(id);

            if (currentUserRole == null)
            {
                throw new Exception("Güncellenecek rol bulunamammıştır.");
            }
            
            var updateModel = new RoleUpdateVM()
            {
                Id = currentUserRole.Id,
                Name = currentUserRole!.Name!,
            };
            
            return View(updateModel);
        }

        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateVM requestModel)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(requestModel.Id);
            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamammıştır.");
            }
            roleToUpdate.Name = requestModel.Name;
            await _roleManager.UpdateAsync(roleToUpdate);
            TempData["SuccessMessage"] = "Rol bilgisi güncellenmiştir";
            return RedirectToAction(nameof(RolesController.Index));
        }

        public async Task<IActionResult> RoleDelete(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);
            if (roleToDelete == null)
            {
                throw new Exception("Silmek için rol bulunamadı");
            }
            var result =  await _roleManager.DeleteAsync(roleToDelete);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.Select(x => x.Description).First());
            }
            TempData["SuccessMessage"] = "Rol başarıyla silindi";
            return RedirectToAction(nameof(RolesController.Index));
        }

    }
}
