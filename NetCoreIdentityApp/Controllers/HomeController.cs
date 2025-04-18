using System.Diagnostics;
using Entities.Concrete;
using Entities.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        returnUrl = returnUrl ?? Url.Action("Index", "Home");
        var hasUser = await _userManager.FindByEmailAsync(model.Email);

        if (hasUser == null)
        {
            ModelState.AddModelError(String.Empty, "Email veya şifre yanlış");
            return View();
        }

        var signInResult = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.RememberMe, false);

        if (signInResult.Succeeded)
        {
            return Redirect(returnUrl);
        }

        ModelState.AddModelErrorList(new List<string>() {"Email veya şifre yanlış"});
        
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
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}