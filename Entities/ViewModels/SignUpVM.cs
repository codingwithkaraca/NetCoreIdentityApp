using System.ComponentModel.DataAnnotations;

namespace Entities.ViewModels;

public class SignUpVM
{
    public SignUpVM()
    {
        
    }
    public SignUpVM(string userName, string email, string phone, string password)
    {
        UserName = userName;
        Email = email;
        Phone = phone;
        Password = password;
    }

    [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz")]
    [Display(Name = "Kullanıcı adı")]
    public string UserName { get; set; } = null!;

    [EmailAddress(ErrorMessage = "Geçerli email formatı giriniz")]
    [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "Telefon alanı boş bırakılamaz")]
    [Display(Name = "Telefon")]
    public string Phone { get; set; }  = null!;
    
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
    [Display(Name = "Şifre")]
    [MinLength(6,ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    public string Password { get; set; } = null!;
    
    [DataType(DataType.Password)]
    [Compare(nameof(Password),ErrorMessage = "Şifreler aynı değil")]
    [Required(ErrorMessage = "Şifre tekrar alanı boş bırakılamaz")]
    [Display(Name = "Şifre Tekrar")]
    [MinLength(6,ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    public string PasswordConfirm { get; set; } = null!;
}