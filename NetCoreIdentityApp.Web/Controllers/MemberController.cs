using System.Security.Claims;
using NetCoreIdentityApp.Entities.Concrete;
using NetCoreIdentityApp.Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using NetCoreIdentityApp.Service.Abstract;
using NetCoreIdentityApp.Service.Concrete;
using NetCoreIdentityApp.Web.Extensions;

namespace NetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IFileProvider _fileProvider;
        private string UserName => User.Identity!.Name!;
        private readonly IMemberService _memberService;

        public MemberController(SignInManager<User> signInManager, UserManager<User> userManager,
            IFileProvider fileProvider, MemberService memberService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
            _memberService = memberService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _memberService.GetUserViewModelByUserNameAsync(UserName));
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
            
            if (! await _memberService.CheckPasswordAsync(UserName, requestModel.PasswordOld))
            {
                ModelState.AddModelError(String.Empty, "Eski şifreniz geçersizdir");
                return View();
            }

            var (isSuccess, errors) =
                await _memberService.ChangePasswordAsync(UserName, requestModel.PasswordOld, requestModel.PasswordNew);
            
            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors!);
                return View();
            }
            
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

            if (requestModel.Picture != null && requestModel.Picture.Length > 0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");
                string randomFileName =
                    $"{Guid.NewGuid().ToString()}{Path.GetExtension(requestModel.Picture.FileName)}";
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

            if (requestModel.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(currentUser, true,
                    new[] { new Claim("birthdate", currentUser.BirthDate.Value.ToString()) });
            }
            else
            {
                await _signInManager.SignInAsync(currentUser, true);
            }
            
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
            await _memberService.LogOutAsync();
        }

        public IActionResult AccessDenied(string ReturnUrl)
        {
            string message = string.Empty;

            message = "Yetkiniz yoktur. Yöneticiniz ile görüşünüz";
            ViewBag.Message = message;
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

        // claim konusunda policy bazlı şehre göre sayfa yetkilendirmek için 
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
            // burada yeni kayıt açan kullanıcıya özel bir sayfayı 10 günlük kullanımı için 
            //UserClaim tablosuna şuanki tarihten 10 gün ekledik
            
            return View();
        }
        
        // policy based authorization ile yaşa göre şiddet sayfasını yetkilendirmek için
        [Authorize(Policy = "ViolencePolicy")]
        [HttpGet]
        public IActionResult ViolencePage()
        {
            return View();
        }
    }
}