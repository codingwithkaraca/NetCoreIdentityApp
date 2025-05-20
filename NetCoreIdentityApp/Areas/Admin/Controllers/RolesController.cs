using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreIdentityApp.Areas.Admin.Models;
using NetCoreIdentityApp.Extensions;

namespace NetCoreIdentityApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
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
        
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            var roleList = await _roleManager.Roles.Select(x => new RoleVM()
            {
                Id = x.Id,
                Name = x.Name!,
            }).ToListAsync();
            return View(roleList);
        }
        
        [Authorize(Roles="Admin")]
        public ActionResult RoleCreate()
        {
            return View();
        }
        
        [Authorize(Roles="Admin")]
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

        [Authorize(Roles="Admin")]
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

        [Authorize(Roles="Admin")]
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

        [Authorize(Roles="Admin")]
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

        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            var currentUser = (await _userManager.FindByIdAsync(id))!;
            ViewBag.userId = currentUser.Id;
            var roles = await _roleManager.Roles.ToListAsync();
            var roleList = new List<AssignRoleToUserVM>();
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            foreach (var role in roles)
            {
                var asssignRoleToUserModel = new AssignRoleToUserVM()
                {
                    Id = role.Id,
                    Name = role.Name!,
                };

                if (userRoles.Contains(role.Name!))
                {
                    asssignRoleToUserModel.IsExists = true ;
                }
                roleList.Add(asssignRoleToUserModel);
            }
            return View(roleList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserVM> requestList)
        {
            var userToAssignRoles = (await _userManager.FindByIdAsync(userId))!;
            foreach (var role in requestList)
            {
                if (role.IsExists)
                {
                    await _userManager.AddToRoleAsync(userToAssignRoles, role.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.Name);
                }
            }
            return RedirectToAction(nameof(HomeController.UserList),"Home");
        }
    }
}
