using System.Diagnostics;
using System.Security.Claims;
using NetCoreIdentityApp.Entities.Concrete;
using NetCoreIdentityApp.Entities.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreIdentityApp.Core.Models;
using NetCoreIdentityApp.Service.Services;
using NetCoreIdentityApp.Web.Extensions;

namespace NetCoreIdentityApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailService _emailService;

    public HomeController(ILogger<HomeController> logger, UserManager<User> userManager,
        SignInManager<User> signInManager, IEmailService emailService)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult SignIn()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignIn(SignInVM model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        returnUrl = returnUrl ?? Url.Action("Index", "Home");
        var hasUser = await _userManager.FindByEmailAsync(model.Email);

        if (hasUser == null)
        {
            ModelState.AddModelError(String.Empty, "Email veya şifre yanlış");
            return View();
        }
        
        var signInResult = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.RememberMe, true);

        if (signInResult.IsLockedOut)
        {
            ModelState.AddModelErrorList(new List<string>()
                { "Hesabınız yanlış denemeden dolayı 3 dakika kitlenmiştir." });
            return View();
        }
        
        if (!signInResult.Succeeded)
        {
            ModelState.AddModelErrorList(new List<string>()
            {
                $"Email veya şifre yanlış", 
                $"Başarısız giriş sayısı = {await _userManager.GetAccessFailedCountAsync(hasUser)}"
            });
            return View();
        }
        
        if (hasUser.BirthDate.HasValue)
        {
            await _signInManager.SignInWithClaimsAsync(hasUser, model.RememberMe, 
                new[] { new Claim("birthdate", hasUser.BirthDate.Value.ToString())});
        }
        return Redirect(returnUrl!);
    }

    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpVM request)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var identityResult = await _userManager.CreateAsync(new User()
        {
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.Phone
        }, request.PasswordConfirm);

        if (!identityResult.Succeeded)
        {
            ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
            return View();
        }

        // burada bu kod claim-bazlı authorizaton daki policy kullanımından dan policy-bazlı Authorization farkını göstermek.
        // kullanıcı kayıt olduğundan sonra 10 günlük bir policy eklemek için, senaryoya göre kullan
        var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());
        var user = await _userManager.FindByNameAsync(request.UserName);
        var claimResult = await _userManager.AddClaimAsync(user!, exchangeExpireClaim);
        if (!claimResult.Succeeded)
        {
            ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
            return View();
        }

        TempData["SuccessMessage"] = "Üyelik kayıt işlemi başarıyla gerçekleşmiştir";
        return RedirectToAction(nameof(HomeController.SignUp));
    }

    public IActionResult ForgetPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordVM requestModel)
    {
        // örnek link : https://localhost:5128?userId=12345&token=adsfasdfsdsdfdgfgdhgdfh
        var hasUser = await _userManager.FindByEmailAsync(requestModel.Email);
        if (hasUser == null)
        {
            ModelState.AddModelError(String.Empty, "Bu emaile sahip kullanıcı bulunamamıştır.");
            return View();
        }

        string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);

        var resetPasswordLink = Url.Action("ResetPassword", "Home",
            new { userId = hasUser.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);

        // Email Service
        await _emailService.SendResetPasswordEmail(resetPasswordLink!, hasUser.Email!);

        TempData["SuccessMessage"] = "Şifre yenileme linki, e-posta adresinize gönderilmiştir";

        return RedirectToAction(nameof(HomeController.ForgetPassword));
    }

    public IActionResult ResetPassword(string userId, string token)
    {
        TempData["userId"] = userId;
        TempData["token"] = token;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordVM requestModel)
    {
        var userId = TempData["userId"];
        var token = TempData["token"];

        if (userId == null || token == null)
        {
            throw new Exception("Bir hata meydana geldi");
        }

        var hasUser = await _userManager.FindByIdAsync(userId.ToString()!);
        if (hasUser == null)
        {
            ModelState.AddModelError(String.Empty, "Kullanıcı bulunamamıştır");
            return View();
        }

        IdentityResult result =
            await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, requestModel.Password);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Şifreniz başarıyla yenilenmiştir";
        }
        else
        {
            ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
        }

        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}