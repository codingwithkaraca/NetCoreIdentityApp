using System.Diagnostics;
using Entities.Concrete;
using Entities.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreIdentityApp.Models;

namespace NetCoreIdentityApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userManager;

    public HomeController(ILogger<HomeController> logger, UserManager<User> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Login()
    {
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

        foreach (var error in identityResult.Errors)
        {
            ModelState.AddModelError(string.Empty,error.Description);
        }
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}