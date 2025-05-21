using Entities.Concrete;
using Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using NetCoreIdentityApp.Extensions;

namespace NetCoreIdentityApp.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IFileProvider _fileProvider;
        public MemberController(SignInManager<User> signInManager, UserManager<User> userManager, IFileProvider fileProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var userViewModel = new UserVM
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                PictureUrl = currentUser.Picture,
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
        public async Task<IActionResult> UserEdit(UserEditVM requestModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
            currentUser.UserName = requestModel.UserName; 
            currentUser.Email = requestModel.Email; 
            currentUser.PhoneNumber = requestModel.Phone; 
            currentUser.BirthDate = requestModel.BirthDate; 
            currentUser.City = requestModel.City; 
            currentUser.Gender = requestModel.Gender; 
            

            if (requestModel.Picture != null && requestModel.Picture.Length > 0 )
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");
                string randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(requestModel.Picture.FileName)}";
                var newPicturePath =
                    Path.Combine(wwwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath!, randomFileName);
                using var stream = new FileStream(newPicturePath, FileMode.Create);
                await requestModel.Picture.CopyToAsync(stream);
                currentUser.Picture = randomFileName;
            }

            var updateToUserResult = await _userManager.UpdateAsync(currentUser);
            if (!updateToUserResult.Succeeded)
            {
                ModelState.AddModelErrorList(updateToUserResult.Errors);
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(currentUser, true);

            TempData["SuccessMessage"] = "Bilgileriniz başarıyla güncellenmiştir";

            var userEditModel = new UserEditVM
            {
                UserName = requestModel.UserName,
                Email = requestModel.Email,
                Phone = requestModel.Phone,
                BirthDate = requestModel.BirthDate,
                City = requestModel.City,
                Gender = requestModel.Gender,
            };
            return View(userEditModel);
        }
        
        // bu çıkış yap butonu çıkış yaptıktan sonra bana hangi url'e gitme avantajını sağlıyor
        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
        }

        public IActionResult AccessDenied(string ReturnUrl)
        {
            string message = string.Empty;

            message = "Yetkiniz yoktur. Yöneticiniz ile görüşünüz";
            ViewBag.Message = message ;
            return View();
        }

        [HttpGet]
        public IActionResult Claims()
        {
            List<ClaimVM> userClaimList = User.Claims.Select(x => new ClaimVM()
            {
                Issuer = x.Issuer,
                Type = x.Type,
                Value = x.Value,
            }).ToList();
            
            return View(userClaimList);
        }

        // claim konusunda policy bazlı sayfa yetkilendirmek için
        [Authorize(Policy = "AnkaraPolicy")]
        [HttpGet]
        public IActionResult AnkaraPage()
        {
            return View();
        }
        
        // policy based authorization ile sayfa yetkilendirmek için
        [Authorize(Policy = "ExchangePolicy")]
        [HttpGet]
        public IActionResult ExchangePage()
        {
            return View();
        }
        
    }
}
