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
    
    [Display(Name = "Kullanıcı adı")]
    public string UserName { get; set; }
    
    [Display(Name = "Email")]
    public string Email { get; set; }
    
    [Display(Name = "Telefon")]
    public string Phone { get; set; }
    
    [Display(Name = "Şifre")]
    public string Password { get; set; }
    
    [Display(Name = "Şifre Tekrar")]
    public string PasswordConfirm { get; set; }
}