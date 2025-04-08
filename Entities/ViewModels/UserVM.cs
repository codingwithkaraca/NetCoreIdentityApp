using System.ComponentModel.DataAnnotations;

namespace Entities.ViewModels;

public class UserVM
{
    public UserVM()
    {
        
    }
    public UserVM(string userName, string email, string phone, string password)
    {
        UserName = userName;
        Email = email;
        Phone = phone;
        Password = password;
    }
    
    [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz")]
    [Display(Name = "Kullanıcı adı")]
    public string UserName { get; set; }
    
    [EmailAddress(ErrorMessage = "Geçerli email formatı giriniz")]
    [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
    [Display(Name = "Email")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Telefon alanı boş bırakılamaz")]
    [Display(Name = "Telefon")]
    public string Phone { get; set; }
    
    
    [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
    [Display(Name = "Şifre")]
    public string Password { get; set; }
    
    [Compare(nameof(Password),ErrorMessage = "Şifreler aynı değil")]
    [Required(ErrorMessage = "Şifre tekrar alanı boş bırakılamaz")]
    [Display(Name = "Şifre Tekrar")]
    public string PasswordConfirm { get; set; }
}