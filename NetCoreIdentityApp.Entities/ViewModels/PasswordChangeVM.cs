using System.ComponentModel.DataAnnotations;

namespace NetCoreIdentityApp.Entities.ViewModels;

public class PasswordChangeVM
{
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Eski şifre boş geçilemez")]
    [Display(Name = "Eski şifre")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    public string PasswordOld { get; set; } = null!;

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Yeni şifre boş geçilemez")]
    [Display(Name = "Yeni şifre")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    public string PasswordNew { get; set; } = null!;

    [DataType(DataType.Password)]
    [Compare(nameof(PasswordNew), ErrorMessage = "Şifreler aynı değil")]
    [Required(ErrorMessage = "Yeni şifre tekrar boş geçilemez")]
    [Display(Name = "Şifre tekrar")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    public string PasswordNewConfirm { get; set; } = null!;
}