using System.ComponentModel.DataAnnotations;

namespace Entities.ViewModels;

public class SignInVM
{

    public SignInVM()
    {
            
    }

    public SignInVM(string email, string password)
    {
        Email = email;
        Password = password;
    }
    
    [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
    [EmailAddress(ErrorMessage = "Geçerli email formatı giriniz")]
    [Display(Name = "Email")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
    [Display(Name = "Şifre")]
    public string Password { get; set; }
    
    [Display(Name = "Beni Hatırla")]
    public bool RememberMe { get; set; }
}
