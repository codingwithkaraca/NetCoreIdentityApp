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
    public string Email { get; set; } = null!;
    
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = null!;
    
    [Display(Name = "Beni Hatırla")]
    public bool RememberMe { get; set; }
}
