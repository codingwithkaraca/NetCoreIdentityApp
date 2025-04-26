using System.ComponentModel.DataAnnotations;

namespace Entities.ViewModels;

public class ResetPasswordVM
{
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
    [Display(Name = "Yeni Şifre")]
    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Şifreler aynı değil")]
    [Required(ErrorMessage = "Şifre tekrar alanı boş bırakılamaz")]
    [Display(Name = "Yeni Şifre Tekrar")]
    public string PasswordConfirm { get; set; } = null!;
}