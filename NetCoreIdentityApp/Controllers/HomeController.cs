using System.Diagnostics;
using Entities.Concrete;
using Entities.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NetCoreIdentityApp.Extensions;
using NetCoreIdentityApp.Models;

namespace NetCoreIdentityApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
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

        if (signInResult.Succeeded)
        {
            return Redirect(returnUrl);
        }

        if (signInResult.IsLockedOut)
        {
            ModelState.AddModelErrorList(new List<string>() {"Hesabınız yanlış denemeden dolayı 3 dakika kitlenmiştir."});
            return View();
        }

        ModelState.AddModelErrorList(new List<string>() {$"Email veya şifre yanlış",$"Başarısız giriş sayısı = {await _userManager.GetAccessFailedCountAsync(hasUser)}"});
        
        return View();
    }
    
    public IActionResult SignUp()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> SignUp(UserVM request)
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

        if (identityResult.Succeeded)
        {
            TempData["SuccessMessage"] = "Üyelik kayıt işlemi başarıyla gerçekleşmiştir";
            return RedirectToAction(nameof(HomeController.SignUp));
        }
        ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
        return View();
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

        string resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);
        var resetPasswordLink = Url.Action("ForgetPassword", "Home",
            new {userId=hasUser.Id, Token = resetPasswordToken});
        
        // Email Service

        TempData["SuccessMessage"] = "Şifre yenileme linki, e-posta adresinize gönderilmiştir";
        
        return RedirectToAction(nameof(HomeController.ForgetPassword));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}